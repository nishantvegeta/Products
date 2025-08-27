using Products.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Products.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ProductsEntityFrameworkCoreModule),
    typeof(ProductsApplicationContractsModule)
    )]
public class ProductsDbMigratorModule : AbpModule
{
}
