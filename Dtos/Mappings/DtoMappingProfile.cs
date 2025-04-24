using AutoMapper;
using CatalogApi.Models;

namespace CatalogApi.Dtos.Mappings;

public class DtoMappingProfile : Profile
{
	public DtoMappingProfile()
	{
		CreateMap<Category, CategoryDto>().ReverseMap();
		CreateMap<Product, ProductDto>().ReverseMap();
	}
}