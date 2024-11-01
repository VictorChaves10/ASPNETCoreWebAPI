using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP.NETCore_WebAPI.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Ocorreu uma exceção não tratada");

            context.Result = new ObjectResult("Houve um problema na sua solicitação, entre em contato com o suporte")
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }
    }
}
