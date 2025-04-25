using Microsoft.Extensions.DependencyInjection;
using CatalogApi.Controllers;

namespace CatalogApiTests;

public class ProductControllerTest
{
	private readonly ProductController? _controller;

	public ProductControllerTest()
	{
		var services = new ServiceCollection();
		services.AddTransient<ProductController>();
		var provider = services.BuildServiceProvider();
		_controller = provider.GetService<ProductController>();
	}

	[Fact]
	public void Test1()
	{

	}
}