using AutoMapper;
using CatalogApi.Dtos;
using CatalogApi.Models;
using CatalogApi.Pagination;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Net.Mime;
using X.PagedList;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ProductController(IUnityOfWork unityOfWork, IMapper mapper) : ControllerBase
{
	private readonly IUnityOfWork _unityOfWork = unityOfWork;

	private readonly IMapper _mapper = mapper;

	private IEnumerable<ProductDto> Paginate(IPagedList<Product> products)
	{
		var metadata = new
		{
			products.Count,
			products.PageSize,
			products.PageCount,
			products.TotalItemCount,
			products.HasNextPage,
			products.HasPreviousPage
		};

		Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
		return _mapper.Map<IEnumerable<ProductDto>>(products);
	}

	[EndpointSummary("Obtém os produtos")]
	[EndpointDescription("Obtém todos os produtos")]
	[ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[Authorize(Policy = "UserOnly")]
	[HttpGet]
	public ActionResult<IEnumerable<ProductDto>> Get()
	{
		var products = _unityOfWork.ProductRepository.GetAllAsync();
		if (products is null)
		{
			return NotFound();
		}
		return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
	}

	[HttpGet("pagination")]
	public async Task<ActionResult<IEnumerable<ProductDto>>> GetAsync([FromQuery] ProductParameteres parameters)
	{
		var products = await _unityOfWork.ProductRepository.GetProductsAsync(parameters);
		return Ok(Paginate(products));
	}

	[HttpGet("filter/price/pagination")]
	public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsFilterPriceAsync([FromQuery] ProductFilterPrice filter)
	{
		var products = await _unityOfWork.ProductRepository.GetProductsFilterPriceAsync(filter);
		return Ok(Paginate(products));
	}

	[EndpointSummary("Obtém um produto")]
	[EndpointDescription("Obtém um produto através do id")]
	[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesDefaultResponseType]
	[HttpGet("{id:int}", Name = "GetProductById")]
	public async Task<ActionResult<ProductDto>> Get(int id)
	{
		var product = await _unityOfWork.ProductRepository.GetAsync(p => p.Id == id);
		if (product is null)
		{
			return NotFound("Product not found");
		}
		var dto = _mapper.Map<ProductDto>(product);
		return Ok(dto);
	}

	[EndpointSummary("Cria um produto")]
	[EndpointDescription("Cria um novo produto")]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesDefaultResponseType]
	[HttpPost]
	public ActionResult<ProductDto> Post(ProductDto dto)
	{
		if (dto is null)
		{
			return BadRequest();
		}
		var product = _unityOfWork.ProductRepository.Create(_mapper.Map<Product>(dto));
		_unityOfWork.CommitAsync();
		return new CreatedAtRouteResult("GetProductById", _mapper.Map<ProductDto>(product));
	}

	[EndpointSummary("Edita um produto")]
	[EndpointDescription("Edita um produto existente")]
	[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesDefaultResponseType]
	[HttpPut]
	public ActionResult<ProductDto> Put([BindRequired] ProductDto dto)
	{
		if (dto is null)
		{
			return BadRequest();
		}
		_unityOfWork.ProductRepository.Update(_mapper.Map<Product>(dto));
		_unityOfWork.CommitAsync();
		return Ok(dto);
	}

	[EndpointSummary("Exclui um produto")]
	[EndpointDescription("Exclui um produto existente")]
	[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesDefaultResponseType]
	[HttpDelete("{id:int}")]
	public async Task<ActionResult<ProductDto>> DeleteAsync(int id)
	{
		var product = await _unityOfWork.ProductRepository.GetAsync(p => p.Id == id);
		if (product is null)
		{
			return NotFound();
		}
		_unityOfWork.ProductRepository.Delete(product);
		await _unityOfWork.CommitAsync();
		return Ok(_mapper.Map<ProductDto>(product));
	}
}