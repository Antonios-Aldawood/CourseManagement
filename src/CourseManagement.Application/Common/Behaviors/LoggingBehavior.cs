using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.Application.Common.Interfaces;
using CourseManagement.Application.Logs.Commands.WriteLog;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IHeaderCarrier
        where TResponse : IErrorOr
{
    private readonly ISender _mediator;

    public LoggingBehavior(ISender mediator)
    {
        _mediator = mediator;
    }

    private Dictionary<string, string>? ExtractHeaders(object request)
    {
        var prop = request.GetType().GetProperty("headers");

        if (prop != null && prop.PropertyType == typeof(Dictionary<string, string>))
        {
            return prop.GetValue(request) as Dictionary<string, string>;
        }

        return null;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response;

        List<string> responseTypes = [];

        var requestType = request.GetType();

        var propertyValues = new List<string>();

        foreach (var prop in requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var name = prop.Name;
            var value = prop.GetValue(request, null);
            string? valueString = null;

            if (value is IEnumerable<int> intList)
            {
                valueString = $"[{string.Join(", ", intList)}]";
            }
            else if (value is IEnumerable<string> stringList)
            {
                valueString = $"[{string.Join(", ", stringList.Select(s => $"\"{s}\""))}]";
            }
            else
            {
                valueString = value?.ToString() ?? null;
            }

            if (name == "password" &&
                !string.IsNullOrEmpty(valueString))
            {
                valueString = new PasswordHasher<string>()
                    .HashPassword(string.Empty, valueString);
            }

            propertyValues.Add($"{name}: {valueString}");
        }

        try
        {
            response = await next();
        }
        catch (Exception ex)
        {
            if (request is not null)
            {
                var UserId = ExtractUserId(request);

                if (UserId.IsError)
                {
                    return (dynamic)UserId.Errors;
                }

                var UserAgent = GetUserAgent(request);

                if (UserAgent.IsError)
                {
                    return (dynamic)UserAgent.Errors;
                }

                responseTypes.Add($"{ex.GetType().Name}: {ex.Message}");

                int statusCode = 500;

                var writeLogResult = await _mediator.Send(new WriteLogCommand(
                    userId: UserId.Value == 0 ? null : UserId.Value,
                    usedEndpoint: GetEndpointName(request),
                    ipAddress: request.GetType().GetProperty("ipAddress")?.GetValue(request)?.ToString(),
                    affectedIds: null,
                    responseTypes: responseTypes,
                    statusCode: statusCode,
                    requestBody: propertyValues,
                    userAgent: UserAgent.Value
                ));

                if (writeLogResult.IsError)
                {
                    return (dynamic)writeLogResult.Errors;
                }
            }

            return (dynamic)Error.Failure(description: $"Logging exception: {ex.Message}");
        }

        if (request is not null)
        {
            List<int>? affectedIds = [];
            int statusCode = 0;

            if (!response.IsError)
            {
                var value = ((dynamic)response).Value;

                switch (value)
                {
                    case IHasAffectedIds single:
                        affectedIds.Add(single.AffectedId);
                        break;

                    case IEnumerable<IHasAffectedIds> collection:
                        affectedIds = collection.Select(x => x.AffectedId).ToList();
                        break;

                    case var paged when IsPagedResult(paged):
                        var itemsProp = paged.GetType().GetProperty("Items");
                        if (itemsProp != null)
                        {
                            var items = itemsProp.GetValue(paged) as IEnumerable<IHasAffectedIds>;
                            if (items != null)
                            {
                                affectedIds = items
                                    .Select(x => x.AffectedId)
                                    .Distinct()
                                    .ToList();
                            }
                        }
                        break;
                }

                responseTypes.Add("Completely successful");

                statusCode = 200;
            }
            else
            {
                foreach (Error error in ((dynamic)response).Errors)
                {
                    responseTypes.Add($"{error.Code}: {error.Description}");

                    switch (error.Code)
                    {
                        case "General.Unauthorized":
                            statusCode = 401;
                            break;
                        case "General.Forbidden":
                            statusCode = 403;
                            break;
                        case "General.NotFound":
                            statusCode = 404;
                            break;
                        case "General.Conflict":
                            statusCode = 409;
                            break;
                        case "General.Unexpected":
                            statusCode = 500;
                            break;
                        case "General.Failure":
                            statusCode = 500;
                            break;

                        default:
                            statusCode = 200;
                            break;
                    }
                }
            }

            var UserId = ExtractUserId(request);

            if (UserId.IsError)
            {
                return (dynamic)UserId.Errors;
            }

            if (UserId == 0 && affectedIds.Count != 0)
            {
                UserId = affectedIds.First();
            }

            var UserAgent = GetUserAgent(request);

            if (UserAgent.IsError)
            {
                return (dynamic)UserAgent.Errors;
            }

            var writeLogResult = await _mediator.Send(new WriteLogCommand(
                userId: UserId.Value == 0 ? null : UserId.Value,
                usedEndpoint: GetEndpointName(request),
                ipAddress: request.GetType().GetProperty("ipAddress")?.GetValue(request)?.ToString(),
                affectedIds: affectedIds,
                responseTypes: responseTypes,
                statusCode: statusCode,
                requestBody: propertyValues,
                userAgent: UserAgent.Value
            ));

            if (writeLogResult.IsError)
            {
                return (dynamic)writeLogResult.Errors;
            }
        }

        return response;
    }

    private ErrorOr<int> ExtractUserId(object request)
    {
        if (request.GetType().Name != "LoginUserCommand" &&
            request.GetType().Name != "RefreshUserTokenCommand")
        {
            try
            {
                var headers = ExtractHeaders(request);

                if (headers == null || !headers.TryGetValue("Authorization", out var authHeader))
                    return Error.Unauthorized(description: "User unauthenticated.");

                string tokenString = authHeader.Trim();

                const string bearerPrefix = "Bearer ";
                if (tokenString != null && tokenString.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    tokenString = tokenString[bearerPrefix.Length..].Trim();
                }
                else
                {
                    return Error.Unexpected(description: "Invalid header.");
                }

                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(tokenString))
                    return Error.Unexpected(description: "User token invalid.");

                var jwtToken = handler.ReadJwtToken(tokenString);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Error.Unexpected(description: "User with no identifier.");
                }

                return userId;
            }
            catch (SecurityTokenException ex)
            {
                return Error.Failure(description: $"Invalid token: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Error.Failure(description: $"{ex.Message}");
            }
        }
        
        return 0;
    }

    private bool IsPagedResult(object value)
    {
        if (value == null)
        {
            return false;
        }

        var type = value.GetType();
        return type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("PagedResult");
    }

    private string GetEndpointName(object request)
    {
        return request.GetType().Name;
    }

    private ErrorOr<string> GetUserAgent(object request)
    {
        //if (request.GetType().Name != "LoginUserCommand" &&
        //    request.GetType().Name != "RefreshUserTokenCommand")
        //{
            var headers = ExtractHeaders(request);
            if (headers is null)
            {
                return Error.Unauthorized(description: "Request is lacking headers.");
            }

            return headers.TryGetValue("User-Agent", out var agent) ? agent : "No agent";
        //}

        //return "Agent unidentified.";
    }
}
