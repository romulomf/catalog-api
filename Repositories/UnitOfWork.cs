using CatalogApi.Context;
using CatalogApi.Models;

namespace CatalogApi.Repositories;

public record UnitOfWork(CatalogApiDbContext Context) : IUnityOfWork, IDisposable
{
	private readonly CatalogApiDbContext Context = Context;

	private ICategoryRepository? _categoryRepository;

	private IProductRepository? _productRepository;

	public ICategoryRepository CategoryRepository => _categoryRepository ??= new CategoryRepository(Context);

	public IProductRepository ProductRepository => _productRepository ??= new ProductRepository(Context);

	public async Task CommitAsync()
	{
		await Context.SaveChangesAsync();
	}

	public void Dispose()
	{
		Context.Dispose();
		GC.SuppressFinalize(this);
	}
}