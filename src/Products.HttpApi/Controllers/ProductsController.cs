using Products.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Products.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ProductsController : AbpControllerBase
{
    protected ProductsController()
    {
        LocalizationResource = typeof(ProductsResource);
    }
}
