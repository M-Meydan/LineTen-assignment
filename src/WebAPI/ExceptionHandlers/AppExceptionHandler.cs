using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebAPI.ExceptionHandlers
{
    public class AppExceptionHandler : IExceptionHandler
    {
        readonly bool _isDebugMode;
        readonly ILogger<AppExceptionHandler> _logger;

        public AppExceptionHandler(IWebHostEnvironment environment, ILogger<AppExceptionHandler> logger)
        {
            _logger = logger;
            _isDebugMode = environment.IsDevelopment();
        }
        public bool CanHandle(ExceptionContext context)
        {
            return context.Exception is ApplicationException;
        }

        public async Task HandleExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception as ApplicationException;

            _logger.LogError(exception, $"Application Exception: {exception.Message}");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Application Error",
                Detail = _isDebugMode ? exception.ToString() : exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            context.Result = new BadRequestObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }
    }
}
