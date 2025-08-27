using System.Threading.Tasks;

namespace Products.Data;

public interface IProductsDbSchemaMigrator
{
    Task MigrateAsync();
}
