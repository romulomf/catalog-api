using System.Text.Json;

namespace CatalogApiClient.Services;

public class CategoryService(IHttpClientFactory httpClientFactory)
{
	private readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};

	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
}