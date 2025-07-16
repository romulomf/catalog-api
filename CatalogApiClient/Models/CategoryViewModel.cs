using System.ComponentModel.DataAnnotations;

namespace CatalogApiClient.Models;

public class CategoryViewModel
{
	public int Id { get; set; }

	[Required(ErrorMessage = "Category name is required.")]
	public string? Name { get; set; }

	[Required]
	[Display(Name = "Image")]
	public string? ImageUrl { get; set; }
}