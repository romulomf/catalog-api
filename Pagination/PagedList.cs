namespace CatalogApi.Pagination;

public class PagedList<T> : List<T> where T: class
{
	public int CurrentPage { get; private set; }

	public int TotalPages { get; private set; }

	public int PageSize { get; private set; }

	public int TotalCount { get; private set; }

	public bool HasPrevious => CurrentPage > 1;

	public bool HasNext => CurrentPage < TotalPages;

	public PagedList(IEnumerable<T> items, int total, int page, int size)
	{
		CurrentPage = page;
		PageSize = size;
		TotalCount = total;
		TotalPages = Convert.ToInt32(Math.Ceiling((double)(total / size)));
		AddRange(items);
	}

	public static PagedList<T> ToPagedList(IQueryable<T> source, int page, int size)
	{
		var count = source.Count();
		IEnumerable<T> items = [.. source.Skip((page - 1) * size).Take(size)];
		return new PagedList<T>(items, count, page, size);
	}
}