using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP.NETCore_WebAPI.Filters
{
    public class ApiLogginFilter : IActionFilter
    {
        private readonly ILogger<ApiLogginFilter> _logger;

        public ApiLogginFilter(ILogger<ApiLogginFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("Executando -> OnActionExecuting");
            
            _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("Executando -> OnActionExecuted");
            _logger.LogInformation($"{DateTime.Now.ToString()}");
            _logger.LogInformation($"Status Code:{context.HttpContext.Response.StatusCode}");
        }


    }
}
