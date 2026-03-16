using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.Customers;
using Products.Dtos;
using Products.Entities.Customers;
using Products.Entities.Orders;
using Products.Orders;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Products.AppServices.Orders;

public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<Customer, Guid> _customerRepository;
    private readonly ILogger<OrderAppService> _logger;
    private readonly IMapper _mapper;

    public OrderAppService(
        IRepository<Order, Guid> orderRepository,
        IRepository<Customer, Guid> customerRepository,
        ILogger<OrderAppService> logger,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ResponseDataDto<object>> CreateAsync(CreateOrderDto input)
    {
        try
        {
            _logger.LogInformation("CreateAsync called with input: {@Input}", input);
            await ValidateCreateInputAsync(input);

            // Verify customer exists
            var customer = await _customerRepository.FindAsync(input.CustomerId);
            if (customer == null)
            {
                throw new UserFriendlyException("Customer not found!");
            }

            var order = _mapper.Map<CreateOrderDto, Order>(input);
            await _orderRepository.InsertAsync(order);

            var dto = _mapper.Map<Order, OrderDto>(order);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Order created successfully",
                Data = dto,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> GetAsync([Required] Guid id)
    {
        try
        {
            var order = await _orderRepository.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Order not found",
                    Data = null,
                    Code = 404
                };
            }

            var dto = _mapper.Map<Order, OrderDto>(order);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Order retrieved successfully",
                Data = dto,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, UpdateOrderDto input)
    {
        try
        {
            await ValidateUpdateInputAsync(input, id);

            var order = await _orderRepository.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Order not found",
                    Data = null,
                    Code = 404
                };
            }

            _mapper.Map(input, order);
            await _orderRepository.UpdateAsync(order);

            var dto = _mapper.Map<Order, OrderDto>(order);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Order updated successfully",
                Data = dto,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id)
    {
        try
        {
            var order = await _orderRepository.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Order not found",
                    Data = null,
                    Code = 404
                };
            }

            await _orderRepository.DeleteAsync(order);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Order deleted successfully",
                Data = null,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting order: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> CancelAsync([Required] Guid id)
    {
        try
        {
            var order = await _orderRepository.FindAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Order not found",
                    Data = null,
                    Code = 404
                };
            }

            order.Status = "cancelled";
            await _orderRepository.UpdateAsync(order);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Order cancelled successfully",
                Data = new { id = order.Id, status = "cancelled" },
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    private async Task ValidateCreateInputAsync(CreateOrderDto input)
    {
        if (input.CustomerId == Guid.Empty)
        {
            throw new UserFriendlyException("Customer ID is required!");
        }

        if (string.IsNullOrWhiteSpace(input.Product))
        {
            throw new UserFriendlyException("Order product cannot be empty!");
        }

        if (input.Quantity <= 0)
        {
            throw new UserFriendlyException("Order quantity must be greater than zero!");
        }

        if (input.Price <= 0)
        {
            throw new UserFriendlyException("Order price must be greater than zero!");
        }

        // Verify customer exists
        var customer = await _customerRepository.FindAsync(input.CustomerId);
        if (customer == null)
        {
            throw new UserFriendlyException("Customer not found!");
        }
    }

    private async Task ValidateUpdateInputAsync(UpdateOrderDto input, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(input.Product))
        {
            throw new UserFriendlyException("Order product cannot be empty!");
        }

        if (input.Quantity <= 0)
        {
            throw new UserFriendlyException("Order quantity must be greater than zero!");
        }

        if (input.Price <= 0)
        {
            throw new UserFriendlyException("Order price must be greater than zero!");
        }

        if (string.IsNullOrWhiteSpace(input.Status))
        {
            throw new UserFriendlyException("Order status is required for update!");
        }
    }
}
