using CatalogApi.Models;
using CatalogApi.Pagination;
using X.PagedList;

namespace CatalogApi.Repositories
{
	public interface IProductRepository : IRepository<Product>
	{
		Task<IPagedList<Product>> GetProductsAsync(ProductParameteres parameters);

		Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductFilterPrice filter);

		Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id);
	}
}