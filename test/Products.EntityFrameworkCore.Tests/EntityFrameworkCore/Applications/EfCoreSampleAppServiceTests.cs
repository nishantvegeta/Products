using Products.Samples;
using Xunit;

namespace Products.EntityFrameworkCore.Applications;

[Collection(ProductsTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<ProductsEntityFrameworkCoreTestModule>
{

}
