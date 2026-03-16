using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Products.Dtos;

namespace Products.Orders;

public interface IOrderAppService : IApplicationService
{
    Task<ResponseDataDto<object>> CreateAsync(CreateOrderDto input);
    Task<ResponseDataDto<object>> GetAsync([Required] Guid id);
    Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, UpdateOrderDto input);
    Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id);
    Task<ResponseDataDto<object>> CancelAsync([Required] Guid id);
}
