using Azure.Core;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseManagement.Api.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        //private Dictionary<string, string> headersDictionary = [];

        //protected void SetHeadersDictionary(Dictionary<string, string> headers)
        //{
        //    headersDictionary = headers;
        //}

        //protected Dictionary<string, string> GetHeadersDictionary()
        //{
        //    return headersDictionary;
        //}

        protected IActionResult Problem(List<Error> errors)
        {
            if (errors.Count is 0)
            {
                return Problem();
            }

            if (errors.All(error => error.Type == ErrorType.Validation))
            {
                //return ValidationProblem(errors);
                return Ok(new
                {
                    Errors = errors.Select(e => new { e.Code, e.Description })
                });
            }

            return Problem(errors[0]);
        }

        protected IActionResult Problem(Error error)
        {
            var statusCode = error.Type switch
            {
                //ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Validation => StatusCodes.Status200OK,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                //ErrorType.Unexpected => StatusCodes.Status502BadGateway,
                ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
                ErrorType.Failure => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError,
            };

            return Problem(statusCode: statusCode, detail: error.Description);
        }

        protected IActionResult ValidationProblem(List<Error> errors)
        {
            var modelStateDictionary = new ModelStateDictionary();

            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(
                    error.Code,
                    error.Description);
            }

            return ValidationProblem(modelStateDictionary);
        }

        protected string GetClientIp()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            /*
            if (HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out Microsoft.Extensions.Primitives.StringValues value))
            {
                ip = value.FirstOrDefault();
            }
            */

            if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            return ip ?? "unknown";
        }
    }
}
