using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.Categories;
using Products.Dtos;
using Products.Entities.Categories;
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

    public CategoryAppService(IRepository<Category, Guid> categoryRepository, ILogger<CategoryAppService> logger, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
        _mapper = mapper;
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
            
            // During development, you might want to see the actual error
            #if DEBUG
                throw new UserFriendlyException($"An unexpected error occurred: {ex.Message}");
            #else
                throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
            #endif
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
