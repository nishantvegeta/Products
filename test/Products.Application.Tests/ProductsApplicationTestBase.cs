using Volo.Abp.Modularity;

namespace Products;

public abstract class ProductsApplicationTestBase<TStartupModule> : ProductsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
