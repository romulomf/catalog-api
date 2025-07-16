using AutoMapper;
using CatalogApi.Dtos;
using CatalogApi.Filters;
using CatalogApi.Models;
using CatalogApi.Pagination;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Mime;
using X.PagedList;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class CategoryController(ILogger<CategoryController> logger, IUnityOfWork unityOfWork, IMapper mapper, IMemoryCache cache) : ControllerBase
{
	private readonly ILogger<CategoryController> _logger = logger;

	private readonly IUnityOfWork _unityOfWork = unityOfWork;

	private readonly IMapper _mapper = mapper;

	private readonly IMemoryCache _cache = cache;

	private const string CacheCategoriesKey = "CacheCategories";

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

	private static string GetCategoryCacheKey(int id) => $"CacheCategory_{id}";

	private void SetCache<T>(string key, T data)
	{
		var cacheOptions = new MemoryCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
			SlidingExpiration = TimeSpan.FromSeconds(15),
			Priority = CacheItemPriority.High,
			Size = data is IEnumerable<Category> collection ? collection.Count() : 1
		};
		_cache.Set(key, data, cacheOptions);
	}

	private void InvalidateCacheAfterChange(int id, Category? category = null)
	{
		var cacheCategoryKey = GetCategoryCacheKey(id);
		_cache.Remove(CacheCategoriesKey);
		_cache.Remove(cacheCategoryKey);
		if (category is not null)
		{
			SetCache(cacheCategoryKey, category);
		}
	}

	[EndpointSummary("Obtém as categorias")]
	[EndpointDescription("Obtém todas as categorias")]
	[HttpGet]
	[ServiceFilter(typeof(ApiLoggingFilter))]
	public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAsync()
	{
		if (!_cache.TryGetValue(CacheCategoriesKey, out IEnumerable<Category>? categories))
		{
			categories = await _unityOfWork.CategoryRepository.GetAllAsync();
			if (categories is not null && categories.Any())
			{
				SetCache(CacheCategoriesKey, categories);
			}
		}
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
		string cacheCategoryKey = GetCategoryCacheKey(id);
		if (!_cache.TryGetValue(cacheCategoryKey, out Category? category))
		{
			category = await _unityOfWork.CategoryRepository.GetAsync(c => c.Id == id);
			if (category is null)
			{
				return NotFound("Category not found");
			}
			SetCache(cacheCategoryKey, category);
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

		InvalidateCacheAfterChange(category.Id, category);

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

		InvalidateCacheAfterChange(category.Id, category);

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

		InvalidateCacheAfterChange(id);

		return Ok(_mapper.Map<CategoryDto>(category));
	}
}