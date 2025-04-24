using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CatalogApi.Filters;

public class ApiExceptionFilter(ILogger<ApiExceptionFilter> logger) : IExceptionFilter
{
	private readonly ILogger<ApiExceptionFilter> _logger = logger;

	public void OnException(ExceptionContext context)
	{
		_logger.LogError(context.Exception, "Ocorreu uma exceção não tratada: Status Code 500");
		context.Result = new ObjectResult("Ocorreu um problema ao tratar a sua solicitação: Status Code 500")
		{
			StatusCode = StatusCodes.Status500InternalServerError,
		};
	}
}