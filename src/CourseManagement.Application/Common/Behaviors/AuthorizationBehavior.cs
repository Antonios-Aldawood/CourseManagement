using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using CourseManagement.Application.Common.Interfaces;

namespace CourseManagement.Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>, IHeaderCarrier
            where TResponse : IErrorOr
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationBehavior(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request.GetType().Name != "LoginUserCommand" &&
                request.GetType().Name != "RefreshUserTokenCommand")
            {
                var headers = request.headers;

                string privilege = typeof(TRequest).Name;

                var isAuthorized = await _authorizationService.IsAuthorized(headers, privilege);

                if (isAuthorized.IsError)
                {
                    return (dynamic)isAuthorized.Errors;
                }
            }
            return await next();
        }
    }
}

/*
    public class AuthorizationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>, IHeaderCarrier
            where TResponse : IErrorOr
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationBehavior(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request.GetType().Name != "LoginUserCommand")
            {
                var user = _httpContextAccessor.HttpContext?.User;

                string privilege = typeof(TRequest).Name;

                var isAuthorized = await _authorizationService.IsAuthorized(user!, privilege);

                if (isAuthorized.IsError)
                {
                    return (dynamic)isAuthorized.Errors;
                }
            }
            return await next();
        }
    }
*/