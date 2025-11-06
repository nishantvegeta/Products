using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Products.BlobDto;
using Products.Dtos;
using Volo.Abp.Application.Dtos;

namespace Products.Categories;

public interface ICategoryAppService
{
    Task<ResponseDataDto<object>> CreateAsync(CreateUpdateCategoryDto input);
    Task<ResponseDataDto<object>> UpdateAsync(Guid id, CreateUpdateCategoryDto input);
    Task<ResponseDataDto<object>> DeleteAsync(Guid id);
    Task<List<DropDownDto>> GetProdcutsAsync();
    Task<ResponseDataDto<PagedResultDto<CategoryDto>>> GetListAsync(PagedAndSortedResultRequestDto input, CategoryFilter filter);
    Task<ResponseDataDto<CategoryDto>> GetAsync(Guid id);
    Task<ResponseDataDto<ExportFileBlobDto>> ExportAsync(CategoryFilter filter);
}
