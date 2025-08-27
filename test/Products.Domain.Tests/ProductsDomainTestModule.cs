using Volo.Abp.Modularity;

namespace Products;

[DependsOn(
    typeof(ProductsDomainModule),
    typeof(ProductsTestBaseModule)
)]
public class ProductsDomainTestModule : AbpModule
{

}
