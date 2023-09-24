using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI.ExceptionHandlers
{
    public class InvalidModelStateExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(ExceptionContext context)
        {
            return !context.ModelState.IsValid;
        }

        public async Task HandleExceptionAsync(ExceptionContext context)
        {
            var details = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Invalid Model State error"
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
    }

}
