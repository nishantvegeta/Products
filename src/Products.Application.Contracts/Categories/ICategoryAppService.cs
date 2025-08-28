using System;
using System.Threading.Tasks;
using Products.Dtos;

namespace Products.Categories;

public interface ICategoryAppService
{
    Task<ResponseDataDto<object>> CreateAsync(CreateUpdateCategoryDto input);
}
