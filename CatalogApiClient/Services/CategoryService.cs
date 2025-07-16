using CatalogApiClient.Models;
using System.Text.Json;

namespace CatalogApiClient.Services;

public class CategoryService(IHttpClientFactory httpClientFactory)
{
	private const string apiEndpoint = "/api/category";

	private readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};

	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync()
	{
		var client = _httpClientFactory.CreateClient("CategoriesApi");
		var response = await client.GetAsync(apiEndpoint);
		if (response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<IEnumerable<CategoryViewModel>>(content, _jsonSerializerOptions) ?? [];
		}
		return [];
	}

	public async Task<CategoryViewModel?> GetCategoryAsync(int id)
	{
		var client = _httpClientFactory.CreateClient("CategoriesApi");
		var response = await client.GetAsync($"{apiEndpoint}/{id}");
		if (response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<CategoryViewModel>(content, _jsonSerializerOptions);
		}
		return null;
	}

	public async Task<CategoryViewModel?> CreateCategoryAsync(CategoryViewModel category)
	{
		var client = _httpClientFactory.CreateClient("CategoriesApi");
		var content = new StringContent(JsonSerializer.Serialize(category, _jsonSerializerOptions), System.Text.Encoding.UTF8, "application/json");
		var response = await client.PostAsJsonAsync(apiEndpoint, content);
		if (response.IsSuccessStatusCode)
		{
			var responseContent = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<CategoryViewModel>(responseContent, _jsonSerializerOptions);
		}
		return null;
	}

	public async Task<CategoryViewModel?> UpdateCategoryAsync(CategoryViewModel category)
	{
		var client = _httpClientFactory.CreateClient("CategoriesApi");
		var content = new StringContent(JsonSerializer.Serialize(category, _jsonSerializerOptions), System.Text.Encoding.UTF8, "application/json");
		var response = await client.PutAsJsonAsync($"{apiEndpoint}/{category.Id}", content);
		if (response.IsSuccessStatusCode)
		{
			var responseContent = await response.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<CategoryViewModel>(responseContent, _jsonSerializerOptions);
		}
		return null;
	}

	public async Task<bool> DeleteCategoryAsync(int id)
	{
		var client = _httpClientFactory.CreateClient("CategoriesApi");
		var response = await client.DeleteAsync($"{apiEndpoint}/{id}");
		return response.IsSuccessStatusCode;
	}
}