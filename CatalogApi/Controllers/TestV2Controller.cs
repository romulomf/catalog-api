using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers
{
	[Route("api/v{v:apiVersion}/test")]
	[ApiController]
	[ApiVersion("2.0")]
	public class TestV2Controller : ControllerBase
	{
		[HttpGet]
		public string GetVersion() => "Teste V2 - GET - Api Versão 2.0";
	}
}