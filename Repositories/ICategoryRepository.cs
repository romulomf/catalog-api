using CatalogApi.Models;
using CatalogApi.Pagination;
using X.PagedList;

namespace CatalogApi.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
	Task<IPagedList<Category>> GetCategoriesAsync(CategoryParameter parameters);

	Task<IPagedList<Category>> GetCategoriesFilterNameAsync(CategoryFilterName filter);
}