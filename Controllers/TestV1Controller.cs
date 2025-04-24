using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace CatalogApi.Controllers
{
	[Route("api/v{v:apiVersion}/test")]
	[ApiController]
	[ApiVersion("1.0")]
	public class TestV1Controller : ControllerBase
	{
		[HttpGet]
		public string GetVersion() => "Teste V1 - GET - Api Versão 1.0";
	}
}