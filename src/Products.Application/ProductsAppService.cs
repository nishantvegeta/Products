using System;
using System.Collections.Generic;
using System.Text;
using Products.Localization;
using Volo.Abp.Application.Services;

namespace Products;

/* Inherit your application services from this class.
 */
public abstract class ProductsAppService : ApplicationService
{
    protected ProductsAppService()
    {
        LocalizationResource = typeof(ProductsResource);
    }
}
