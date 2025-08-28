using AutoMapper;
using Products.Categories;
using Products.Entities.Categories;
using Products.Entities.Products;
using Products.Products;

namespace Products;

public class ProductsApplicationAutoMapperProfile : Profile
{
    public ProductsApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        
        // Category mappings
        CreateMap<CreateUpdateCategoryDto, Category>();
        CreateMap<Category, CategoryDto>();
        
        // Product mappings (if you have them)
        CreateMap<CreateUpdateProductDto, Product>();
        CreateMap<Product, ProductDto>();
    }
}
