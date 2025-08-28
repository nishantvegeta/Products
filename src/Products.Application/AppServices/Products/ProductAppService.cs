using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.Dtos;
using Products.Entities.Categories;
using Products.Entities.Products;
using Products.Products;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;

namespace Products.AppServices.Products;

public class ProductAppService : ApplicationService, IProductAppService
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly ILogger<ProductAppService> _logger;
    private readonly IMapper _mapper;
    private readonly IRepository<Category, Guid> _categoryRepository;

    public ProductAppService(IRepository<Product, Guid> productRepository, ILogger<ProductAppService> logger, IMapper mapper)
    {
        _productRepository = productRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ResponseDataDto<object>> CreateAsync(CreateUpdateProductDto input)
    {
        try
        {
            _logger.LogInformation("CreateAsync method called with input: {@Input}", input);

            await ValidateInputAsync(input);

            var product = _mapper.Map<CreateUpdateProductDto, Product>(input);
            await _productRepository.InsertAsync(product);

            var query = _mapper.Map<Product, ProductDto>(product);

            var categoriesQueryable = await _categoryRepository.GetQueryableAsync();

            var category = await AsyncExecuter.FirstOrDefaultAsync(categoriesQueryable.Where(c => c.Id == query.CategoryId));

            var result = new ProductDto
            {
                Id = query.Id,
                Name = query.Name,
                Description = query.Description,
                Price = query.Price,
                StockQuantity = query.StockQuantity,
                CategoryId = query.CategoryId,
                CategoryName = category?.Name
            };

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Product created successfully",
                Data = result,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "A user-friendly error occurred while creating a product: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a product.");
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");

        }
    }

    public async Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, CreateUpdateProductDto input)
    {
        try
        {
            _logger.LogInformation("UpdateAsync method called with id: {Id} and input: {@Input}", id, input);

            await ValidateInputAsync(input, id);

            var product = await _productRepository.GetAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Product not found",
                    Data = null,
                    Code = 404
                };
            }

            _mapper.Map(input, product);
            await _productRepository.UpdateAsync(product);

            var result = _mapper.Map<Product, ProductDto>(product);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = result,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "A user-friendly error occurred while updating a product: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating a product.");
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id)
    {
        try
        {
            _logger.LogInformation("DeleteAsync method called with id: {Id}", id);

            var product = await _productRepository.GetAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id {Id} not found.", id);
                return new ResponseDataDto<object>
                {
                    Success = false,
                    Message = "Product not found",
                    Data = null,
                    Code = 404
                };
            }

            await _productRepository.DeleteAsync(product);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Product deleted successfully",
                Data = null,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "A user-friendly error occurred while deleting a product: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting a product.");
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }


    public async Task<ResponseDataDto<object>> GetAsync([Required] Guid id)
    {
        try
        {
            var product = await _productRepository.GetAsync(id);

            var products = await _productRepository.GetQueryableAsync();
            var categories = await _categoryRepository.GetQueryableAsync();

            var query = from p in products
                        join c in categories on p.CategoryId equals c.Id into pc
                        from c in pc.DefaultIfEmpty()
                        where p.Id == id
                        select new ProductDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            StockQuantity = p.StockQuantity,
                            CategoryId = p.CategoryId,
                            CategoryName = c != null ? c.Name : null
                        };

            var result = await AsyncExecuter.FirstOrDefaultAsync(query);

            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Product retrieved successfully",
                Data = query,
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "A user-friendly error occurred while retrieving a product: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving a product.");
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    public async Task<ResponseDataDto<object>> GetListAsync(PagedAndSortedResultRequestDto input, ProductFilter filter)
    {
        try
        {
            _logger.LogInformation("GetListAsync method called with input: {@Input} and filter: {@Filter}", input, filter);

            filter.SearchKeyword = filter.SearchKeyword?.Trim()?.ToLower();

            var products = await _productRepository.GetQueryableAsync();
            var categories = await _categoryRepository.GetQueryableAsync();

            var query = (from p in products
                         join c in categories on p.CategoryId equals c.Id
                         select new ProductDto
                         {
                             Id = p.Id,
                             Name = p.Name,
                             Description = p.Description,
                             Price = p.Price,
                             StockQuantity = p.StockQuantity,
                             CategoryId = p.CategoryId,
                             CategoryName = c.Name
                         })
                        .WhereIf(!string.IsNullOrWhiteSpace(filter.SearchKeyword),
                            x => x.Name.ToLower().Contains(filter.SearchKeyword) ||
                                 x.Description.ToLower().Contains(filter.SearchKeyword) ||
                                 x.CategoryName.ToLower().Contains(filter.SearchKeyword)
                        );

            var items = await AsyncExecuter.ToListAsync
                (query
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                );

            var totalCount = await AsyncExecuter.CountAsync(query);


            return new ResponseDataDto<object>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = new PagedResultDto<ProductDto>(totalCount, items),
                Code = 200
            };
        }
        catch (UserFriendlyException ex)
        {
            _logger.LogWarning(ex, "A user-friendly error occurred while retrieving products: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products.");
            throw new UserFriendlyException("An unexpected error occurred. Please try again later.");
        }
    }

    private async Task ValidateInputAsync(CreateUpdateProductDto input, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new UserFriendlyException("Product name cannot be empty!");
        }

        if (input.Price <= 0)
        {
            throw new UserFriendlyException("Product price must be greater than zero!");
        }

        if (input.StockQuantity < 0)
        {
            throw new UserFriendlyException("Stock quantity cannot be negative!");
        }

        // Check category exists
        var categoriesQueryable = await _categoryRepository.GetQueryableAsync();

        var categoryExists = await AsyncExecuter.AnyAsync(
            categoriesQueryable.Where(c => c.Id == input.CategoryId)
        );

        if (!categoryExists)
        {
            throw new UserFriendlyException("CategoryNotFound");
        }

        // Example uniqueness check for Name
        var productsQueryable = await _productRepository.GetQueryableAsync();

        var existing = await AsyncExecuter.FirstOrDefaultAsync(
            productsQueryable.Where(p => p.Name == input.Name && (!id.HasValue || p.Id != id.Value))
        );

        if (existing != null)
        {
            throw new UserFriendlyException("Product name must be unique!");
        }
    }
}
