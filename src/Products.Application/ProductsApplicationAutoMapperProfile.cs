using AutoMapper;
using Products.Categories;
using Products.Customers;
using Products.Entities.Categories;
using Products.Entities.Customers;
using Products.Entities.Orders;
using Products.Entities.Products;
using Products.Orders;
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

        // Product mappings
        CreateMap<CreateUpdateProductDto, Product>();
        CreateMap<Product, ProductDto>();

        // Customer mappings
        CreateMap<CreateUpdateCustomerDto, Customer>();
        CreateMap<Customer, CustomerDto>();

        // Order mappings
        CreateMap<CreateOrderDto, Order>();
        CreateMap<UpdateOrderDto, Order>();
        CreateMap<Order, OrderDto>();
    }
}
