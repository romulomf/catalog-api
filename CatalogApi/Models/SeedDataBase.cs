using CatalogApi.Context;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Models;

public static class SeedDataBase
{
	public static void Populate(CatalogApiDbContext context)
	{
		context.Database.Migrate();
		if (!context.Categories.Any())
		{
			Category c1 = new() { Name = "Bebidas", ImageUrl = "bebidas.jpg" };
			Category c2 = new() { Name = "Lanches", ImageUrl = "lanches.jpg" };
			Category c3 = new() { Name = "Sobremesas", ImageUrl = "sobremesas.jpg" };
			Category c4 = new() { Name = "Frutas", ImageUrl = "frutas.jpg" };
			Category c5 = new() { Name = "Salgados", ImageUrl = "salgados.jpg" };
			Category c6 = new() { Name = "Doces", ImageUrl = "doces.jpg" };

			context.Categories.AddRange(c1, c2, c3, c4, c5, c6);
			context.SaveChanges();
			
			if (!context.Products.Any())
			{
				Product p1 = new()
				{
					Name = "Coca-Cola Diet",
					Description = "Refrigerante de Cola 350ml",
					ImageUrl = "cocacoladiet.jpg",
					Category = c1,
					Price = 5.45m,
					Stock = 50d,
					RegisterDate = DateTime.Now
				};

				Product p2 = new()
				{
					Name = "Lanche de Atum",
					Description = "Lanche de Atum com Maionese",
					ImageUrl = "atum.jpg",
					Category = c2,
					Price = 8.50m,
					Stock = 10d,
					RegisterDate = DateTime.Now
				};

				Product p3 = new()
				{
					Name = "Pudim 100g",
					Description = "Pudim de Leite Condensado",
					ImageUrl = "pudim.jpg",
					Category = c3,
					Price = 6.75m,
					Stock = 20d,
					RegisterDate = DateTime.Now
				};

				Product p4 = new()
				{
					Name = "Maçã",
					Description = "Maçã Fuji",
					ImageUrl = "maca.jpg",
					Category = c4,
					Price = 1.75m,
					Stock = 12d,
					RegisterDate = DateTime.Now
				};

				Product p5 = new()
				{
					Name = "Pastel de Carne",
					Description = "Massa caseira, carne, ovo e tempeiro verde",
					ImageUrl = "pastel.jpg",
					Category = c5,
					Price = 8.95m,
					Stock = 3d,
					RegisterDate = DateTime.Now
				};

				Product p6 = new()
				{
					Name = "Brigadeiro",
					Description = "Chocolate e leite condensado",
					ImageUrl = "brigadeiro.jpg",
					Category = c6,
					Price = 2.50m,
					Stock = 4d,
					RegisterDate = DateTime.Now
				};

				context.Products.AddRange(p1, p2, p3, p4, p5, p6);
				context.SaveChanges();
			}
		}
	}
}