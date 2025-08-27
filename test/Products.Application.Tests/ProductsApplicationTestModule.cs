using Volo.Abp.Modularity;

namespace Products;

[DependsOn(
    typeof(ProductsApplicationModule),
    typeof(ProductsDomainTestModule)
)]
public class ProductsApplicationTestModule : AbpModule
{

}
