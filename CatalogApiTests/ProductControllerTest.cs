using AutoMapper;
using CatalogApi.Controllers;
using CatalogApi.Dtos;
using CatalogApi.Models;
using CatalogApi.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CatalogApiTests;

public class ProductControllerTest
{
	private readonly Mock<IUnityOfWork> _mockUnitOfWork;

	private readonly Mock<IMapper> _mockMapper;

	private readonly ProductController _controller;

	public ProductControllerTest()
	{
		_mockUnitOfWork = new Mock<IUnityOfWork>();
		_mockMapper = new Mock<IMapper>();

		_controller = new ProductController(_mockUnitOfWork.Object, _mockMapper.Object);
	}

	[Fact]
	public async Task GetByIdTest()
	{
		Product productModel = new()
		{
			Id = 1,
			Name = "Product 1",
			Description = "Description of Product 1",
			ImageUrl = "product1.jpg",
			Price = 12.5m
		};

		ProductDto productDto = new()
		{
			Id = 1,
			Name = "Product 1",
			Description = "Description of Product 1",
			ImageUrl = "product1.jpg",
			Price = 12.5m
		};

		var mockProductRepository = new Mock<IProductRepository>();
		mockProductRepository.Setup(d => d.GetAsync(p => p.Id == 1)).ReturnsAsync(productModel);
		_mockUnitOfWork.Setup(d => d.ProductRepository).Returns(mockProductRepository.Object);

		_mockMapper.Setup(d => d.Map<ProductDto>(productModel)).Returns(productDto);

		ActionResult<ProductDto> products = await _controller.Get(1);

		// verifica se há a invocação de pelo menos uma vez do repositório de produtos
		_mockUnitOfWork.Verify(d => d.ProductRepository, Times.Once());
		// verifica se há a invocação de pelo menos uma vez do auto mapper
		_mockMapper.Verify(d => d.Map<ProductDto>(It.IsAny<Product>()), Times.Once());

		products.Result.Should()
			.BeOfType<OkObjectResult>()
			.Which.StatusCode.Should().Be(200);
	}
}