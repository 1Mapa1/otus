using Microsoft.AspNetCore.Mvc;
using WarehouseService.Application.Common;

namespace WarehouseService.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return ToErrorResult(result.Error);
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return ToErrorResult(result.Error);
        }

        private static IActionResult ToErrorResult(Error? error)
        {
            if (error is null)
            {
                return new ObjectResult(new
                {
                    errorCode = "UnknownError",
                    message = "Unknown error."
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            var statusCode = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var body = error.Details ?? new
            {
                errorCode = error.Code,
                message = error.Message
            };

            return new ObjectResult(body)
            {
                StatusCode = statusCode
            };
        }
    }
}
