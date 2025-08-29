using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Products.BlobDto;
using Products.Dtos;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Products.Products;

public interface IProductAppService
{
    Task<ResponseDataDto<object>> CreateAsync(CreateUpdateProductDto input);
    Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, CreateUpdateProductDto input);
    Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id);
    Task<ResponseDataDto<object>> GetAsync([Required] Guid id);
    Task<ResponseDataDto<object>> GetListAsync(PagedAndSortedResultRequestDto input, ProductFilter filter);
    Task<ResponseDataDto<ExportFileBlobDto>> ExportAsync(ProductFilter filter);
    Task<ResponseDataDto<object>> BulkImportAsync([FromForm] BulkImportFileDto input);
}
