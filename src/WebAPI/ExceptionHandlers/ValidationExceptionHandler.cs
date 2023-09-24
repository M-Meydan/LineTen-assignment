using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.ExceptionHandlers
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(ExceptionContext context)
        {
            return context.Exception is ValidationException;
        }

        public async Task HandleExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;

            var errors = exception.Errors
                .GroupBy(failure => failure.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).ToArray());

            var details = new ValidationProblemDetails(errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Validation error"
            };

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }
    }
}
