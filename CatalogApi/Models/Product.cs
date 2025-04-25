using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Models;

[Table("PRODUCTS")]
[PrimaryKey(nameof(Id))]
public class Product
{
	[Key]
	[Column("ID", TypeName = "INT")]
	public int Id { get; set; }

	[Required]
	[Column("NAME", TypeName = "NVARCHAR(MAX)")]
	public string? Name { get; set; }

	[Required]
	[Column("DESCRIPTION", TypeName = "NVARCHAR(MAX)")]
	public string? Description { get; set; }

	[Required]
	[Column("PRICE", TypeName = "SMALLMONEY")]
	public decimal Price { get; set; }

	[Required]
	[Column("IMAGE_URL", TypeName = "NVARCHAR(MAX)")]
	public string? ImageUrl { get; set; }

	[Column("STOCK", TypeName = "DECIMAL(10,2)")]
	public double Stock { get; set; }

	[Column("REGISTER_DATE", TypeName = "SMALLDATETIME")]
	public DateTime RegisterDate { get; set; }

	[JsonIgnore]
	public Category? Category { get; set; }
}