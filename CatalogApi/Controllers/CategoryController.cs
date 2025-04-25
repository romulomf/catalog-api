using AutoMapper;
using CatalogApi.Dtos;
using CatalogApi.Filters;
using CatalogApi.Models;
using CatalogApi.Pagination;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Net.Mime;
using X.PagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class CategoryController(ILogger<CategoryController> logger, IUnityOfWork unityOfWork, IMapper mapper) : ControllerBase
{
	private readonly ILogger<CategoryController> _logger = logger;

	private readonly IUnityOfWork _unityOfWork = unityOfWork;

	private readonly IMapper _mapper = mapper;

	private IEnumerable<CategoryDto> Paginate(IPagedList<Category> categories)
	{
		var metadata = new
		{
			categories.Count,
			categories.PageSize,
			categories.PageCount,
			categories.TotalItemCount,
			categories.HasNextPage,
			categories.HasPreviousPage
		};

		Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
		return _mapper.Map<IEnumerable<CategoryDto>>(categories);
	}

	[EndpointSummary("Obtém as categorias")]
	[EndpointDescription("Obtém todas as categorias")]
	[Authorize]
	[HttpGet]
	[ServiceFilter(typeof(ApiLoggingFilter))]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAsync()
	{
		var categories = await _unityOfWork.CategoryRepository.GetAllAsync();
		return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
	}

	[HttpGet("pagination")]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAsync([FromQuery] CategoryParameter parameters)
	{
		var categories = await _unityOfWork.CategoryRepository.GetCategoriesAsync(parameters);
		return Ok(Paginate(categories));
	}

	[HttpGet("filter/name/pagination")]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesFilterNameAsync([FromQuery] CategoryFilterName filter)
	{
		var categories = await _unityOfWork.CategoryRepository.GetCategoriesFilterNameAsync(filter);
		return Ok(Paginate(categories));
	}

	[EndpointSummary("Obtém uma categoria")]
	[EndpointDescription("Obtém categoria através do id")]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
	[ProducesDefaultResponseType]
	[HttpGet("{id:int}", Name = "GetCategoryById")]
	public async Task<ActionResult<CategoryDto>> Get(int id)
	{
		var category = await _unityOfWork.CategoryRepository.GetAsync(c => c.Id == id);
		if (category is null)
		{
			return NotFound("Category not found");
		}
		return Ok(_mapper.Map<CategoryDto>(category));
	}

	[EndpointSummary("Cria uma categoria")]
	[EndpointDescription("Cria uma nova categoria")]
	[Produces(MediaTypeNames.Application.Json)]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
	[ProducesDefaultResponseType]
	[HttpPost]
	public ActionResult<CategoryDto> Post(CategoryDto dto)
	{
		if (dto is null)
		{
			return BadRequest();
		}
		var category = _unityOfWork.CategoryRepository.Create(_mapper.Map<Category>(dto));
		_unityOfWork.CommitAsync();

		return new CreatedAtRouteResult("GetCategoryById", _mapper.Map<CategoryDto>(category));
	}

	[EndpointSummary("Edita uma categoria")]
	[EndpointDescription("Edita uma categoria existente")]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
	[ProducesDefaultResponseType]
	[HttpPut]
	public ActionResult<CategoryDto> Put(CategoryDto dto)
	{
		if (dto is null)
		{
			return BadRequest();
		}
		var category = _unityOfWork.CategoryRepository.Update(_mapper.Map<Category>(dto));
		_unityOfWork.CommitAsync();
		return Ok(_mapper.Map<CategoryDto>(category));
	}

	[EndpointSummary("Exclui uma categoria")]
	[EndpointDescription("Exclui uma categoria existente")]
	[Authorize(Policy = "AdminOnly")]
	[ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(NotFound), StatusCodes.Status404NotFound)]
	[ProducesDefaultResponseType]
	[HttpDelete("{id:int}")]
	public async Task<ActionResult<CategoryDto>> DeleteAsync(int id)
	{
		var category = await _unityOfWork.CategoryRepository.GetAsync(c => c.Id == id);
		if (category is null)
		{
			return NotFound();
		}
		category = _unityOfWork.CategoryRepository.Delete(category);
		await _unityOfWork.CommitAsync();
		return Ok(_mapper.Map<CategoryDto>(category));
	}
}