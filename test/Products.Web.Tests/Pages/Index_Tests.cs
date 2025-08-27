using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Products.Pages;

public class Index_Tests : ProductsWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
