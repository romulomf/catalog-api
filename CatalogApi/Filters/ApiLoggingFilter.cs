using Microsoft.AspNetCore.Mvc.Filters;

namespace CatalogApi.Filters;

public class ApiLoggingFilter(ILogger<ApiLoggingFilter> logger) : IActionFilter
{
	private readonly ILogger<ApiLoggingFilter> _logger = logger;

	public void OnActionExecuted(ActionExecutedContext context)
	{
		_logger.LogInformation("Executando -> OnActionExecuted");
		_logger.LogInformation(DateTime.Now.ToLongTimeString());
		_logger.LogInformation($"Status Code: {context.HttpContext.Response.StatusCode}");
	}

	public void OnActionExecuting(ActionExecutingContext context)
	{
		_logger.LogInformation("Executando -> OnActionExecuting");
		_logger.LogInformation(message: DateTime.Now.ToLongTimeString());
	}
}