using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Shared.CommonResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Commerce.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                if(result.Value != null)
                {
                    return Ok(result.Value);
                }
                return NoContent();
            }
            else
            {
                return HandleProblem(result.Errors);
            }
        }
        protected ActionResult<T> HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return HandleProblem(result.Errors);
            }
        }


        private ActionResult HandleProblem(IReadOnlyList<Error> errors)
        {
            // No Error
            if (errors.Count == 0)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "An Unexpected Error Happened");
            }
            // valdation Error
            if (errors.All(e => e.Type == ErrorType.Validation))
            {
                return HandleValidationProblem(errors);
            }
            // single Error
            return HandleSingleErrorProblem(errors[0]);
        }
        private ActionResult HandleSingleErrorProblem(Error error)
        {
            return Problem(
                title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                statusCode: MapErrorTypeToStatusCode(error.Type)
            );
        }
        private static int MapErrorTypeToStatusCode(ErrorType errortype) => errortype switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.InvalidCrendentials => StatusCodes.Status401Unauthorized,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
        private ActionResult HandleValidationProblem(IReadOnlyList<Error> errors)
        {
            var modelstate = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelstate.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelstate);
        }
    }
}
