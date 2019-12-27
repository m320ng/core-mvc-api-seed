using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SeedApi.Controllers {
    [ApiController]
    public class ErrorController : ControllerBase {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger) {
            _logger = logger;
        }

        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment) {
            if (webHostEnvironment.EnvironmentName != "Development") {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            _logger.LogError(context.Error, "Error");

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}