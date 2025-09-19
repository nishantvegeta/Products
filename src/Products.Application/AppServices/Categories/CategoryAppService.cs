using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.Categories;
using Products.Dtos;
using Products.Entities.Categories;
using Products.Entities.Products;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.Threading;

namespace Products.AppServices.Categories;

public class CategoryAppService : ApplicationService, ICategoryAppService
{
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly ILogger<CategoryAppService> _logger;
    private readonly IMapper _mapper;
    private readonly IRepository<Product, Guid> _productRepository;

    public CategoryAppService(IRepository<Category, Guid> categoryRepository, ILogger<CategoryAppService> logger, IMapper mapper, IRepository<Product, Guid> productRepository)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
        _mapper = mapper;
        _productRepository = productRepository;
    }

    public async Task<ResponseDataDto<object>> CreateAsync(CreateUpdateCategoryDto input)
    {
        try
        {
            await ValidateInputAsync(input);

            var category = _mapper.Map<CreateUpdateCategoryDto, Category>(input);
            await _categoryRepository.InsertAsync(category);

            var result = _mapper.Map<Category, CategoryDto>(category);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Category created successfully",
                Data = result,
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
            _logger.LogError(ex, "An unexpected error occurred while creating a category: {ErrorDetails}", ex.ToString());
            throw new UserFriendlyException($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<ResponseDataDto<object>> UpdateAsync(Guid id, CreateUpdateCategoryDto input)
    {
        try
        {
            var category = await _categoryRepository.GetAsync(id);
            if (category == null)
                throw new UserFriendlyException("Category not found!");

            await ValidateInputAsync(input);

            _mapper.Map(input, category);
            await _categoryRepository.UpdateAsync(category);

            var result = _mapper.Map<Category, CategoryDto>(category);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = result,
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
            _logger.LogError(ex, "An unexpected error occurred while updating a category: {ErrorDetails}", ex.ToString());
            throw new UserFriendlyException($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<ResponseDataDto<object>> DeleteAsync(Guid id)
    {
        try
        {
            var category = await _categoryRepository.GetAsync(id);
            if (category == null)
                throw new UserFriendlyException("Category not found!");

            await _categoryRepository.DeleteAsync(category);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Category deleted successfully",
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
            _logger.LogError(ex, "An unexpected error occurred while deleting a category: {ErrorDetails}", ex.ToString());
            throw new UserFriendlyException($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<List<DropDownDto>> GetProdcutsAsync()
    {
        try
        {
            var products = await _productRepository.GetQueryableAsync();

            var result = products.Select(p => new DropDownDto
            {
                Value = p.Id.ToString(),
                Name = p.Name
            }).ToList();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving products: {ErrorDetails}", ex.ToString());
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    private async Task ValidateInputAsync(CreateUpdateCategoryDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            throw new UserFriendlyException("Category name cannot be empty!");

        if (string.IsNullOrWhiteSpace(input.Code))
            throw new UserFriendlyException("Category code cannot be empty!");

        var queryable = await _categoryRepository.GetQueryableAsync();

        var existing = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(c => c.Code == input.Code));

        if (existing != null)
            throw new UserFriendlyException("Category code must be unique!");
    }
}
