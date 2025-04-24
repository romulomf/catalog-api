using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CatalogApi.Dtos;

public class CategoryDto
{
	[Description("O id exclusivo")]
	public int Id { get; set; }

	[Description("O nome da categoria")]
	[Required]
	public string? Name { get; set; }

	[Description("A URL da imagem que simboliza a categoria")]
	[Required]
	public string? ImageUrl { get; set; }
}