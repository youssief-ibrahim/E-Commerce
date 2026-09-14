using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Factory
{
    public class ApiResponseFactory
    {
        public static IActionResult GenerateApiValidationResponse(ActionContext actionContext)
        {
            var errors = actionContext.ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .ToDictionary(i => i.Key, i => i.Value!.Errors.Select(i => i.ErrorMessage).ToArray());
            var errorResponse = new ProblemDetails()
            {
                Title = "One or more validation errors occurred.",
                Detail = "See the Errors property for details.",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "Errors", errors } }
            };
            return new BadRequestObjectResult(errorResponse);
        }
    }
}
