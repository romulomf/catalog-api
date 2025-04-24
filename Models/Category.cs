using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Models;

[Table("CATEGORIES")]
[PrimaryKey(nameof(Id))]
public class Category
{
	[Key]
	[Column("ID", TypeName = "INT")]
	public int Id { get; set; }

	[Required]
	[Column("NAME", TypeName = "NVARCHAR(MAX)")]
	public string? Name { get; set; }

	[Required]
	[Column("IMAGE_URL", TypeName = "NVARCHAR(MAX)")]
	public string? ImageUrl { get; set; }

	public ICollection<Product>? Products { get; set; }
}