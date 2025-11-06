using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.Categories;
using Products.Dtos;
using Products.Entities.Categories;
using Products.Entities.Products;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Volo.Abp.Uow;
using Microsoft.AspNetCore.Mvc;
using Products.BlobDto;
using Products.ExportServices;

namespace Products.AppServices.Categories;

public class CategoryAppService : ApplicationService, ICategoryAppService
{
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly ILogger<CategoryAppService> _logger;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IMapper _mapper;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IExportService _exportService;

    public CategoryAppService(
        IRepository<Category, Guid> categoryRepository,
        ILogger<CategoryAppService> logger,
        IRepository<Product, Guid> productRepository,
        IUnitOfWorkManager unitOfWorkManager,
        IExportService exportService)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
        _productRepository = productRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _exportService = exportService;
    }

    public async Task<ResponseDataDto<object>> CreateAsync(CreateUpdateCategoryDto input)
    {
        try
        {
            await ValidateInputAsync(input);

            using var uow = _unitOfWorkManager.Begin();
            var category = new Category
            {
                Name = input.Name,
                Code = input.Code,
                Description = input.Description
            };

            await _categoryRepository.InsertAsync(category);

            await uow.CompleteAsync();

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Category created successfully",
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
            using var uow = _unitOfWorkManager.Begin();

            await ValidateInputAsync(input);
            var existingCategory = await GetCategoryAsync(id);

            existingCategory.Name = input.Name;
            existingCategory.Code = input.Code;
            existingCategory.Description = input.Description;

            await _categoryRepository.UpdateAsync(existingCategory);

            await uow.CompleteAsync();

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Category updated successfully",
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

    public async Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id)
    {
        try
        {
            using var uow = _unitOfWorkManager.Begin();

            var category = await GetCategoryAsync(id);
            if (category == null)
                throw new UserFriendlyException("Category not found!");

            await _categoryRepository.DeleteAsync(category);

            await uow.CompleteAsync();

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

    public async Task<ResponseDataDto<PagedResultDto<CategoryDto>>> GetListAsync(PagedAndSortedResultRequestDto input, CategoryFilter filter)
    {
        try
        {
            using var uow = _unitOfWorkManager.Begin();
            var category = await _categoryRepository.GetQueryableAsync();

            if (string.IsNullOrWhiteSpace(input.Sorting))
            {
                input.Sorting = "Name";
            }

            filter.SearchKeyword = filter.SearchKeyword?.Trim()?.ToLower();

            var query = category.Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description
            })
                .WhereIf(!string.IsNullOrWhiteSpace(filter.SearchKeyword),
                    x => x.Name.ToLower().Contains(filter.SearchKeyword) ||
                        x.Code.ToLower().Contains(filter.SearchKeyword)
                );

            var items = await AsyncExecuter.ToListAsync(query.OrderBy(input.Sorting).Skip(input.SkipCount).Take(input.MaxResultCount));

            var totalCount = await AsyncExecuter.CountAsync(query);

            var result = new PagedResultDto<CategoryDto>(totalCount, items);
            return new ResponseDataDto<PagedResultDto<CategoryDto>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = result,
                Code = 200
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving categories: {ErrorDetails}", ex.ToString());
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<CategoryDto>> GetAsync(Guid id)
    {
        try
        {
            using var uow = _unitOfWorkManager.Begin();

            var category = await _categoryRepository.FirstOrDefaultAsync(s => s.Id == id);

            var query = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                Description = category.Description
            };

            return new ResponseDataDto<CategoryDto>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = query,
                Code = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving a category: {ErrorDetails}", ex.ToString());
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    [HttpGet]
    public async Task<ResponseDataDto<ExportFileBlobDto>> ExportAsync(CategoryFilter filter)
    {
        try
        {
            _logger.LogInformation("ExportAsync method called with filter: {@Filter}", filter);

            filter.SearchKeyword = filter.SearchKeyword?.Trim()?.ToLower();

            var products = await _productRepository.GetQueryableAsync();
            var categories = await _categoryRepository.GetQueryableAsync();

            var query = categories
            .WhereIf(!string.IsNullOrWhiteSpace(filter.SearchKeyword),
                x => x.Name.ToLower().Contains(filter.SearchKeyword) ||
                     x.Code.ToLower().Contains(filter.SearchKeyword)
            )
            .Select(x => new CategoryExportDto
            {
                Name = x.Name,
                Code = x.Code,
                Description = x.Description
            });

            var items = await AsyncExecuter.ToListAsync(query);

            var exportFormat = string.IsNullOrWhiteSpace(filter.ExportFormat)
            ? FileTypes.JsonType
            : filter.ExportFormat.ToLowerInvariant();

            // export using service
            var fileBytes = await _exportService.ExportAsync(items, exportFormat);

            var fileBlob = new ExportFileBlobDto
            {
                Name = $"Category_{DateTime.UtcNow:yyyyMMddHHmmss}{exportFormat}",
                Content = fileBytes
            };

            return new ResponseDataDto<ExportFileBlobDto>
            {
                Success = true,
                Message = "Products exported successfully",
                Data = fileBlob,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "A user-friendly error occurred while exporting products: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while exporting products.");
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
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


    #region Private Methods

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

    private async Task<Category> GetCategoryAsync(Guid id)
    {
        var category = await _categoryRepository.FindAsync(s => s.Id == id);
        return category is null
            ? throw new UserFriendlyException("Category not found!")
            : category;
    }



    #endregion
}
