using CatalogApi.Context;
using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CatalogApi.Repositories
{
	public record Repository<T>(CatalogApiDbContext Context) : IRepository<T> where T : class
	{
		protected readonly CatalogApiDbContext Context = Context;

		public T Create(T entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			Context.Set<T>().Add(entity);
			return entity;
		}

		public T Delete(T entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			Context.Set<T>().Remove(entity);
			return entity;
		}

		public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate) => await Context.Set<T>().FirstOrDefaultAsync(predicate);

		public async Task<IEnumerable<T>> GetAllAsync() => await Context.Set<T>().AsNoTracking().ToListAsync();

		public T Update(T entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			Context.Set<T>().Update(entity);
			return entity;
		}
	}
}