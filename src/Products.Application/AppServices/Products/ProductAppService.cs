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
using Products.BlobDto;
using Microsoft.AspNetCore.Mvc;
using Products.ExportServices;
using System.IO;
using Products.Constants;
using Products.BulkImport;
using Volo.Abp.Validation;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Products.AppServices.Products;

public class ProductAppService : ApplicationService, IProductAppService
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly ILogger<ProductAppService> _logger;
    private readonly IMapper _mapper;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IExportService _exportService;
    private readonly IBulkImportService _bulkImportService;

    public ProductAppService(
        IRepository<Product, Guid> productRepository,
        ILogger<ProductAppService> logger,
        IMapper mapper,
        IRepository<Category, Guid> categoryRepository,
        IExportService exportService,
        IBulkImportService bulkImportService)
    {
        _productRepository = productRepository;
        _logger = logger;
        _mapper = mapper;
        _categoryRepository = categoryRepository;
        _exportService = exportService;
        _bulkImportService = bulkImportService;
    }

    private readonly string _validationTitle = "Bulk Import Product error Validations";

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

    [HttpGet]
    public async Task<ResponseDataDto<ExportFileBlobDto>> ExportAsync(ProductFilter filter)
    {
        try
        {
            _logger.LogInformation("ExportAsync method called with filter: {@Filter}", filter);

            filter.SearchKeyword = filter.SearchKeyword?.Trim()?.ToLower();

            var products = await _productRepository.GetQueryableAsync();
            var categories = await _categoryRepository.GetQueryableAsync();

            var query = (from p in products
                         join c in categories on p.CategoryId equals c.Id
                         select new ProductExportDto
                         {
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

            var items = await AsyncExecuter.ToListAsync(query);

            var exportFormat = string.IsNullOrWhiteSpace(filter.ExportFormat)
            ? FileTypes.JsonType
            : filter.ExportFormat.ToLowerInvariant();

            // export using service
            var fileBytes = await _exportService.ExportAsync(items, exportFormat);

            var fileBlob = new ExportFileBlobDto
            {
                Name = $"Products_{DateTime.UtcNow:yyyyMMddHHmmss}{exportFormat}",
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

    public async Task<ResponseDataDto<object>> BulkImportAsync([FromForm] BulkImportFileDto input)
    {
        _logger.LogInformation("BulkImportAsync: Started");
        try
        {
            var extension = new FileInfo(input.File.FileName).Extension;
            if (!BulkImportFileConsts.ExtensionAllowed.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase)))
            {
                var msg = "Invalid File Extension. Allowed extensions are: " + string.Join(", ", BulkImportFileConsts.ExtensionAllowed);
                _logger.LogWarning("BulkImportAsync: File extension validation failed. File name: {FileName}, Extension: {Extension}, Message: {Message}",
                    input.File.FileName, extension, msg);
                throw new UserFriendlyException(msg, "400");
            }

            if (input.File.Length > BulkImportFileConsts.FileSizeLimit)
            {
                var msg = "File Size Exceeded. Maximum allowed size is " + (BulkImportFileConsts.FileSizeLimit / (1024 * 1024)) + " MB.";
                _logger.LogWarning("BulkImportAsync: File size validation failed. File name: {FileName}, File size: {FileSize}, Message: {Message}",
                    input.File.FileName, input.File.Length, msg);
                throw new UserFriendlyException(msg, "400");
            }

            using var stream = new MemoryStream();
            await input.File.CopyToAsync(stream);
            stream.Position = 0;

            var products = await _bulkImportService.GetDataAsync<ProductImportDto>(stream, extension);
            if (products == null || products.Count == 0)
            {
                var msg = "The uploaded file does not contain any ApiClient data.";
                _logger.LogWarning("BulkImportAsync: No data found in the file");
                throw new UserFriendlyException(msg, "404");
            }

            await BulkImportAsync(products);

            _logger.LogInformation("BulkImportAsync: Process completed successfully");

            return new ResponseDataDto<object>
            {
                Code = 200,
                Success = true,
                Message = "Bulk import successful",
            };
        }
        catch (AbpValidationException ex)
        {
            Logger.LogError(ex, "BulkImportAsync: Validation failed");

            // Optionally: Log individual validation errors
            foreach (var ve in ex.ValidationErrors)
            {
                Logger.LogWarning("ValidationError: {ErrorMessage}", ve.ErrorMessage);
            }

            // Re-throw as-is so ABP can convert to proper RemoteServiceErrorInfo
            throw;
        }
        catch (Exception ex) when (ex is not UserFriendlyException)
        {
            Logger.LogError("::Error::" + ex.Message);
            throw new UserFriendlyException("Bulk import failed. " + ex.Message);
        }
    }

    private async Task BulkImportAsync(List<ProductImportDto> input)
    {
        _logger.LogInformation("BulkImportAsync: Starting import of ApiClients");

        try
        {
            if (!input.Any())
            {
                var msg = "Please provide at least one ApiClient record.";
                _logger.LogInformation("BulkImportAsync: {Validation} - {Message}", _validationTitle, msg);
                throw new AbpValidationException(_validationTitle,
                                                [
                                                    new(msg, ["itemCount"])
                                                ]);
            }

            var newProducts = new List<Product>();
            var updateProducts = new List<Product>();

            var validations = new List<ValidationResult>();

            // Get existing ApiClients from repository
            var existingApiClients = await _productRepository.GetListAsync();

            foreach (var (item, index) in input.Select((value, idx) => (value, idx)))
            {
                int rowNum = index + 1;

                item.Name = item.Name?.Trim();

                // Validate required SystemName
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    var message = $"Row {rowNum}: Name (Name) is required.";
                    _logger.LogInformation("BulkImportAsync: {Message}", message);
                    validations.Add(new ValidationResult(message, ["Name"]));
                }

                // Validate SystemName format
                if (!string.IsNullOrWhiteSpace(item.Name) && item.Name.Length > ProductConsts.MaxLength.MaxNameLength)
                {
                    var message = $"Row {rowNum}: Name (Name) must not exceed {ProductConsts.MaxLength.MaxNameLength}";
                    _logger.LogInformation("BulkImportAsync: {Message}", message);
                    validations.Add(new(message, ["Name.Length"]));
                }

                // Check duplicates in input list
                var duplicatesNames = input
                    .Where((v, i) => i != index &&
                                    !string.IsNullOrWhiteSpace(v.Name) &&
                                    v.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (duplicatesNames.Any())
                {
                    var message = $"Row {rowNum}: Duplicate Name '{item.Name}' found in import data.";
                    _logger.LogInformation("BulkImportAsync: {Message}", message);
                    validations.Add(new ValidationResult(message, new[] { "Name" }));
                }

                // Validate SystemName does not contain spaces
                if (item.Name.Contains(" "))
                {
                    var message = $"Row {rowNum}: Name '{item.Name}' contains space";
                    item.Name = Regex.Replace(item.Name.Trim(), @"\s+", "_");
                }

                // Find existing entity by SystemName
                var existingEntity = existingApiClients
                .FirstOrDefault(e => e.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));

                if (existingEntity == null)
                {

                    if (validations.Count == 0)
                    {
                        var newEntity = new Product
                        {
                            Name = item.Name,
                            Description = item.Description,
                            Price = item.Price,
                            StockQuantity = item.StockQuantity,
                            IsActive = item.IsActive,
                            ExpiryDate = item.ExpiryDate,
                            CategoryId = item.CategoryId ?? Guid.Empty, // or handle as needed
                            // Add any defaults or additional properties as needed
                        };
                        newProducts.Add(newEntity);
                    }
                }
                else
                {
                    if (validations.Count == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Name))
                        {
                            existingEntity.Name = item.Name;
                        }

                        updateProducts.Add(existingEntity);
                    }
                }
            }

            if (validations.Count != 0)
            {
                throw new AbpValidationException(_validationTitle, validations);
            }

            if (newProducts.Count != 0)
            {
                await _productRepository.InsertManyAsync(newProducts);
                _logger.LogInformation("BulkImportAsync: Inserted {Count} new ApiClients", newProducts.Count);
            }

            if (updateProducts.Count != 0)
            {
                await _productRepository.UpdateManyAsync(updateProducts);
                _logger.LogInformation("BulkImportAsync: Updated {Count} ApiClients", updateProducts.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BulkImportAsync: Error occurred during bulk import");
            throw;
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
