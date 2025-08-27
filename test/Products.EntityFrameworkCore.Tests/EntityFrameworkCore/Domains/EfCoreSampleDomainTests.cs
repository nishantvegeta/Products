using Products.Samples;
using Xunit;

namespace Products.EntityFrameworkCore.Domains;

[Collection(ProductsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<ProductsEntityFrameworkCoreTestModule>
{

}
