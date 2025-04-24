using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CatalogApi.Dtos;

public class ProductDto
{
	[Description("O id exclusivo")]
	public int Id { get; set; }

	[Description("O nome do produto")]
	[Required]
	public string? Name { get; set; }

	[Description("A descrição textual do produto")]
	[Required]
	public string? Description { get; set; }

	[Description("O preço do produto")]
	[Required]
	public decimal Price { get; set; }

	[Description("A URL da imagem que ilustra o produto")]
	[Required]
	public string? ImageUrl { get; set; }
}