using CatalogApi.Context;
using CatalogApi.Dtos;
using CatalogApi.Models;
using CatalogApi.Pagination;
using System.Reflection;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace CatalogApi.Repositories;

public record ProductRepository(CatalogApiDbContext Context) : Repository<Product>(Context), IProductRepository
{
	public async Task<IPagedList<Product>> GetProductsAsync(ProductParameteres parameters)
	{
		var products = await GetAllAsync();
		return products.OrderBy(p => p.Name).AsQueryable().ToPagedList(parameters.Page, parameters.Size);
	}

	public async Task<IPagedList<Product>> GetProductsFilterPriceAsync(ProductFilterPrice filter)
	{
		var products = await GetAllAsync();

		if (filter.Price.HasValue && !string.IsNullOrEmpty(filter.Criteria))
		{
			var criteria = filter.Criteria.ToLowerInvariant();
			products = criteria switch
			{
				"major" => products.Where(p => p.Price > filter.Price.Value).OrderBy(p => p.Name),
				"minor" => products.Where(p => p.Price < filter.Price.Value).OrderBy(p => p.Name),
				_ => products.Where(p => p.Price == filter.Price.Value).OrderBy(p => p.Name),
			};
		}
		
		return products.AsQueryable().ToPagedList(filter.Page, filter.Size);
	}

	public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int id)
	{
		var products = await GetAllAsync();
		return products.Where(p => p.Category?.Id == id);
	}
}