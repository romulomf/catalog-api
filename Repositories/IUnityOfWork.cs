using CatalogApi.Models;

namespace CatalogApi.Repositories;

public interface IUnityOfWork
{
	ICategoryRepository CategoryRepository { get; }

	IProductRepository ProductRepository { get; }

	Task CommitAsync();
}