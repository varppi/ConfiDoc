using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Confidoc.Server.Helpers
{
    /// <summary>
    /// Premade IActionResult HTTP responses to reduce code duplication.
    /// </summary>
    public static class HttpStatus
    {
        public static IActionResult FieldErrors(
            Dictionary<string, IEnumerable<string>> errors,
            int statusCode=500)
        {
            var errorResponse =  new JsonResult(new{errors})
            {
                StatusCode = statusCode
            };
            return errorResponse;
        }

        public static IActionResult IdentityErrors(
            IdentityResult result, 
            string targetVariable,
            int statusCode=500)
        {
            Dictionary<string, IEnumerable<string>> errors = new()
            {
                { targetVariable, result.Errors.Select(x => x.Description) }
            };
            
            var errorResponse =  new JsonResult(new{errors})
            {
                StatusCode = statusCode
            };
            return errorResponse;
        }

        public static readonly IActionResult Login = new RedirectResult("/login");

        public static readonly IActionResult BadRequest = new JsonResult(new
        {
            Message = "bad request. make sure all fields are correctly filled."
        })
        {
            StatusCode = 400
        };

        public static readonly IActionResult NotFoundAllowed = new JsonResult(new
        {
            Message = "not found or no read permissions"
        })
        {
            StatusCode = 404
        };

        public static readonly IActionResult Forbidden = new JsonResult(new
        {
            Message = "user does not have permissions to execute this operation"
        })
        {
            StatusCode = 403
        };

        public static readonly IActionResult Internal = new JsonResult(new
        {
            Message = "something went wrong"
        })
        {
            StatusCode = 500
        };

        public static readonly IActionResult Ok = new JsonResult(new
        {
            Message = "Ok"
        })
        {
            StatusCode = 200
        };
    }
}
