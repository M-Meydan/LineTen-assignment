using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.ExceptionHandlers;

namespace WebAPI.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;

        public ApiExceptionFilterAttribute(IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            _exceptionHandlers = exceptionHandlers;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            await HandleExceptionAsync(context);
            base.OnException(context);
        }

        private async Task HandleExceptionAsync(ExceptionContext context)
        {
            foreach (var handler in _exceptionHandlers)
            {
                if (handler.CanHandle(context))
                {
                    await handler.HandleExceptionAsync(context);
                    return;
                }
            }
        }
    }





}
//    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
//    {

//        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

//        public ApiExceptionFilterAttribute()
//        {
//            // Register known exception types and handlers.
//            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
//            {
//                { typeof(ValidationException), HandleValidationException }
//            };
//        }

//        public override void OnException(ExceptionContext context)
//        {
//            HandleException(context);

//            base.OnException(context);
//        }

//        private void HandleException(ExceptionContext context)
//        {
//            Type type = context.Exception.GetType();
//            if (_exceptionHandlers.ContainsKey(type))
//            {
//                _exceptionHandlers[type].Invoke(context);
//                return;
//            }

//            if (!context.ModelState.IsValid)
//            {
//                HandleInvalidModelStateException(context);
//                return;
//            }

//            HandleUnknownException(context);
//        }

//        private void HandleValidationException(ExceptionContext context)
//        {
//            var exception = (ValidationException)context.Exception;
//            var errors = exception.Errors.GroupBy(failure => failure.PropertyName)
//                .ToDictionary(group => group.Key,
//                              group => group.Select(failure => failure.ErrorMessage).ToArray());

//            var details = new ValidationProblemDetails(errors)
//            {
//                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
//                Title = "Validation error"
//            };

//            context.Result = new BadRequestObjectResult(details);

//            context.ExceptionHandled = true;
//        }

//        private void HandleInvalidModelStateException(ExceptionContext context)
//        {
//            var details = new ValidationProblemDetails(context.ModelState)
//            {
//                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
//            };

//            context.Result = new BadRequestObjectResult(details);

//            context.ExceptionHandled = true;
//        }


//        private void HandleUnknownException(ExceptionContext context)
//        {
//            var details = new ProblemDetails
//            {
//                Status = StatusCodes.Status500InternalServerError,
//                Title = "An error occurred while processing your request.",
//                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
//            };

//            context.Result = new ObjectResult(details)
//            {
//                StatusCode = StatusCodes.Status500InternalServerError
//            };

//            context.ExceptionHandled = true;
//        }
//    }
//}
