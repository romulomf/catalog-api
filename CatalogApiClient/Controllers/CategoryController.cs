using CatalogApiClient.Services;
using CatalogApiClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApiClient.Controllers;

public class CategoryController(CategoryService categoryService) : Controller
{
	private readonly CategoryService _categoryService = categoryService;

	public async Task<ActionResult<IEnumerable<CategoryViewModel>>> Index()
	{
		var categories = await _categoryService.GetCategoriesAsync();
		if (categories is not null)
		{
			return View(categories);
		}
		return NotFound("Categories not found.");
	}
}