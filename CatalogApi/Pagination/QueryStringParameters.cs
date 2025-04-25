namespace CatalogApi.Pagination;

public abstract class QueryStringParameters
{
	const int maxPageSize = 50;

	private int _pageSize = maxPageSize;
	
	public int Page { get; set; } = 1;
	
	public int Size
	{
		get => _pageSize;
		set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
	}
}
