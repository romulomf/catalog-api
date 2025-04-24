namespace CatalogApi.Pagination;

public class ProductFilterPrice : QueryStringParameters
{
	public decimal? Price { get; set; }

	public string? Criteria { get; set; }
}