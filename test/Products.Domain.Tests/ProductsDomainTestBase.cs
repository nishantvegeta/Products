using Volo.Abp.Modularity;

namespace Products;

/* Inherit from this class for your domain layer tests. */
public abstract class ProductsDomainTestBase<TStartupModule> : ProductsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
