using System.Reflection.Metadata;
using X.PagedList;
using X.PagedList.Extensions;
using CatalogApi.Context;
using CatalogApi.Models;
using CatalogApi.Pagination;

namespace CatalogApi.Repositories;

public record CategoryRepository(CatalogApiDbContext Context) : Repository<Category>(Context), ICategoryRepository
{
	public async Task<IPagedList<Category>> GetCategoriesAsync(CategoryParameter parameters)
	{
		var categories = await GetAllAsync();
		return categories.OrderBy(p => p.Name).AsQueryable().ToPagedList(parameters.Page, parameters.Size);
	}

	public async Task<IPagedList<Category>> GetCategoriesFilterNameAsync(CategoryFilterName filter)
	{
		var categories = await GetAllAsync();

		if (!string.IsNullOrEmpty(filter.Name))
		{
			categories = categories.Where(c => c.Name != null && c.Name.Contains(filter.Name));
		}

		return categories.AsQueryable().ToPagedList(filter.Page, filter.Size);
	}
}