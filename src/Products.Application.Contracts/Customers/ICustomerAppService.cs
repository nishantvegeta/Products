using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Products.Dtos;

namespace Products.Customers;

public interface ICustomerAppService : IApplicationService
{
    Task<ResponseDataDto<object>> CreateAsync(CreateUpdateCustomerDto input);
    Task<ResponseDataDto<object>> GetAsync([Required] Guid id);
    Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, CreateUpdateCustomerDto input);
    Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id);
    Task<ResponseDataDto<object>> ActivateAsync([Required] Guid id);
}
