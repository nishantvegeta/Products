using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.Customers;
using Products.Dtos;
using Products.Entities.Customers;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Products.AppServices.Customers;

public class CustomerAppService : ApplicationService, ICustomerAppService
{
    private readonly IRepository<Customer, Guid> _customerRepository;
    private readonly ILogger<CustomerAppService> _logger;
    private readonly IMapper _mapper;

    public CustomerAppService(
        IRepository<Customer, Guid> customerRepository,
        ILogger<CustomerAppService> logger,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ResponseDataDto<object>> CreateAsync(CreateUpdateCustomerDto input)
    {
        try
        {
            _logger.LogInformation("CreateAsync called with input: {@Input}", input);
            await ValidateInputAsync(input);

            var customer = _mapper.Map<CreateUpdateCustomerDto, Customer>(input);
            await _customerRepository.InsertAsync(customer);

            var dto = _mapper.Map<Customer, CustomerDto>(customer);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Customer created successfully",
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
            _logger.LogError(ex, "Error creating customer: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> GetAsync([Required] Guid id)
    {
        try
        {
            var customer = await _customerRepository.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Customer not found",
                    Data = null,
                    Code = 404
                };
            }

            var dto = _mapper.Map<Customer, CustomerDto>(customer);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Customer retrieved successfully",
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
            _logger.LogError(ex, "Error retrieving customer: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, CreateUpdateCustomerDto input)
    {
        try
        {
            await ValidateInputAsync(input, id);

            var customer = await _customerRepository.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Customer not found",
                    Data = null,
                    Code = 404
                };
            }

            _mapper.Map(input, customer);
            await _customerRepository.UpdateAsync(customer);

            var dto = _mapper.Map<Customer, CustomerDto>(customer);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Customer updated successfully",
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
            _logger.LogError(ex, "Error updating customer: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id)
    {
        try
        {
            var customer = await _customerRepository.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Customer not found",
                    Data = null,
                    Code = 404
                };
            }

            await _customerRepository.DeleteAsync(customer);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Customer deleted successfully",
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
            _logger.LogError(ex, "Error deleting customer: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> ActivateAsync([Required] Guid id)
    {
        try
        {
            var customer = await _customerRepository.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Customer with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Customer not found",
                    Data = null,
                    Code = 404
                };
            }

            customer.IsActive = true;
            await _customerRepository.UpdateAsync(customer);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Customer activated successfully",
                Data = new { id = customer.Id, status = "activated" },
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
            _logger.LogError(ex, "Error activating customer: {ErrorMessage}", ex.Message);
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    private async Task ValidateInputAsync(CreateUpdateCustomerDto input, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new UserFriendlyException("Customer name cannot be empty!");
        }

        if (string.IsNullOrWhiteSpace(input.Email))
        {
            throw new UserFriendlyException("Customer email cannot be empty!");
        }

        if (string.IsNullOrWhiteSpace(input.Phone))
        {
            throw new UserFriendlyException("Customer phone cannot be empty!");
        }

        // Check email uniqueness
        var queryable = await _customerRepository.GetQueryableAsync();
        var existing = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(c => c.Email == input.Email && (!id.HasValue || c.Id != id.Value))
        );
        if (existing != null)
        {
            throw new UserFriendlyException("Customer email must be unique!");
        }
    }
}
