using Microsoft.AspNetCore.Builder;
using Products;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Products.Web.csproj");
await builder.RunAbpModuleAsync<ProductsWebTestModule>(applicationName: "Products.Web" );

public partial class Program
{
}
