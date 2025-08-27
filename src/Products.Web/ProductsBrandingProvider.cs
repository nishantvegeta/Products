using Microsoft.Extensions.Localization;
using Products.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Products.Web;

[Dependency(ReplaceServices = true)]
public class ProductsBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<ProductsResource> _localizer;

    public ProductsBrandingProvider(IStringLocalizer<ProductsResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
