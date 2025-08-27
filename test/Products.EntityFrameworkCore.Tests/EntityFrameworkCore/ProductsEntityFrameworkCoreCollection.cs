using Xunit;

namespace Products.EntityFrameworkCore;

[CollectionDefinition(ProductsTestConsts.CollectionDefinitionName)]
public class ProductsEntityFrameworkCoreCollection : ICollectionFixture<ProductsEntityFrameworkCoreFixture>
{

}
