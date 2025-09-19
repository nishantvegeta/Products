using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Products.BulkImport;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using CsvHelper;
using System.Globalization;
using System.Linq;
using Ganss.Excel;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System.Text.Json;
using Newtonsoft.Json;
using Products.Constants;

namespace Products.AppServices.BulkImport;

public class BulkImportService(
    ILogger<BulkImportService> logger
    ) : IBulkImportService, ITransientDependency
{
    public async Task<List<T>> GetDataAsync<T>(MemoryStream stream, string extension) where T : IBulkImportDto
    {
        try
        {
            logger.LogInformation("BulkImportService: GetDataAsync - Started");
            stream.Position = 0;

            if (extension.Equals(FileType.CsvType, StringComparison.OrdinalIgnoreCase))
            {
                using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    HeaderValidated = null,
                    MissingFieldFound = null,
                });

                var records = csv.GetRecords<T>()
                    .Where(emp => !string.IsNullOrWhiteSpace(emp.DataIdentifier))
                .ToList();

                logger.LogInformation("BulkImportService: GetDataAsync - Inserted data successfully");
                return records;

            }
            else if (extension.Equals(FileType.XlsxType, StringComparison.OrdinalIgnoreCase))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var importer = new ExcelMapper(stream);

                var records = importer.Fetch<T>()
                    .Where(emp => !string.IsNullOrWhiteSpace(emp.DataIdentifier))
                    .ToList();

                logger.LogInformation("BulkImportService: GetDataAsync - Inserted data successfully");
                return records;
            }
            else if (extension.Equals(FileType.JsonType, StringComparison.OrdinalIgnoreCase))
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var jsonContent = await reader.ReadToEndAsync();

                var jsonObject = JObject.Parse(jsonContent);
                var dtoName = typeof(T).Name;

                var items = jsonObject["items"]?.ToObject<List<T>>();

                if (items == null)
                {
                    logger.LogError("BulkImportService: GetDataAsync - Failed to parse JSON file");
                    throw new UserFriendlyException("Invalid JSON format. Expected structure: { \"items\": [...] }", "400");
                }

                logger.LogInformation("BulkImportService: GetDataAsync - Parsed JSON with {Count} records", items.Count);
                return items;
            }
            else
            {
                logger.LogInformation("BulkImportService: GetDataAsync - Unsupported file format");
                throw new UserFriendlyException("Unsupported file format.", "400");
            }
        }
        catch (UserFriendlyException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "BulkImportService: GetDataAsync - error occured");
            throw;
        }
    }

    public async Task<List<T>> GetDataAsync<T>(IFormFile file, string tableIdentifier) where T : IBulkImportDto
    {
        try
        {
            logger.LogInformation("BulkImportService: GetDataAsync - Started");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var extension = ValidateExcelFileAsync(file);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (extension.Equals(FileType.CsvType, StringComparison.OrdinalIgnoreCase))
            {
                var records = await ExtractCsvDataAsync<T>(stream, tableIdentifier);

                return records;
            }
            else if (extension.Equals(FileType.XlsxType, StringComparison.OrdinalIgnoreCase))
            {
                var records = await ExtractXlsxData<T>(stream, tableIdentifier);

                return records;
            }
            else if (extension.Equals(FileType.JsonType, StringComparison.OrdinalIgnoreCase))
            {
                var records = await ExtractJsonDataAsync<T>(stream, tableIdentifier);
                return records;
            }
            else
            {
                logger.LogInformation("BulkImportService: GetDataAsync - Unsupported file format");
                throw new UserFriendlyException("Unsupported file format.", "400");
            }
        }
        catch (UserFriendlyException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "BulkImportService: GetDataAsync - error occured");
            throw;
        }
    }

    public async Task<T> GetJsonDataAsync<T>(IFormFile file) where T : class
    {
        try
        {
            logger.LogInformation("GetJsonDataAsync: Started");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            return await GetJsonDataAsync<T>(stream);
        }
        catch (UserFriendlyException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetJsonDataAsync: Error occurred");
            throw;
        }
    }

    private async Task<T> GetJsonDataAsync<T>(MemoryStream stream) where T : class
    {
        try
        {
            logger.LogInformation("GetJsonDataAsync: Started");

            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var jsonContent = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                logger.LogWarning("GetJsonDataAsync: JSON content is empty");
                throw new UserFriendlyException("JSON file is empty.", "400");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<T>(jsonContent, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                if (result == null)
                {
                    logger.LogWarning("GetJsonDataAsync: Failed to deserialize JSON");
                    throw new UserFriendlyException("Invalid JSON format.", "400");
                }

                logger.LogInformation("GetJsonDataAsync: JSON deserialized successfully");
                return result;
            }
            catch (System.Text.Json.JsonException ex)
            {
                logger.LogError(ex, "GetJsonDataAsync: JSON parsing error");
                throw new UserFriendlyException("Invalid JSON format. Please check the file structure.", "400");
            }
        }
        catch (UserFriendlyException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetJsonDataAsync: Error occurred");
            throw;
        }
    }

    private string ValidateExcelFileAsync(IFormFile file)
    {
        var extension = new FileInfo(file.FileName).Extension;

        if (!BulkImportFileConsts.ExtensionAllowed.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase)))
        {
            var msg = "Invalid File Extension. Allowed extensions are: " + string.Join(", ", BulkImportFileConsts.ExtensionAllowed);
            logger.LogWarning("ApplicationAppService - BulkImportAsync: File extension validation failed. File name: {FileName}, Extension: {Extension}, Message: {Message}",
                file.FileName, extension, msg);
            throw new UserFriendlyException(msg, "400");
        }

        if (file.Length > BulkImportFileConsts.FileSizeLimit)
        {
            var msg = "File Size Exceeded. Maximum allowed size is " + (BulkImportFileConsts.FileSizeLimit / (1024 * 1024)) + " MB.";
            logger.LogWarning("ApplicationAppService - BulkImportAsync: File size validation failed. File name: {FileName}, File size: {FileSize}, Message: {Message} ",
                file.FileName, file.Length, msg);
            throw new UserFriendlyException(msg, "400");
        }

        return extension;
    }

    private async Task<List<T>> ExtractCsvDataAsync<T>(Stream stream, string tableIdentifier)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        using var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
        });

        await csv.ReadAsync();
        csv.ReadHeader();

        var headers = csv.HeaderRecord?.ToList() ?? new List<string>();

        var records = new List<T>();
        while (await csv.ReadAsync())
        {
            var record = csv.GetRecords<T>();
        }

        logger.LogInformation("BulkImportService: GetDataAsync - Inserted data successfully");
        return records;
    }

    private async Task<List<T>> ExtractXlsxData<T>(Stream stream, string tableIdentifier)
    {
        return await Task.Run(() =>
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var headers = new List<string>();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new UserFriendlyException("No worksheet found in the Excel file.");
                }

                int colCount = worksheet.Dimension.Columns;
                for (int col = 1; col <= colCount; col++)
                {
                    headers.Add(worksheet.Cells[1, col].Text);
                }
            }

            var importer = new ExcelMapper(stream);
            var records = importer.Fetch<T>()
                //.Where(emp => !string.IsNullOrWhiteSpace(emp.DataIdentifier))
                .ToList();

            logger.LogInformation("BulkImportService: GetDataAsync - Inserted data successfully");
            return records;
        });
    }

    private async Task<List<T>> ExtractJsonDataAsync<T>(Stream stream, string tableIdentifier)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var jsonContent = await reader.ReadToEndAsync();

        var jsonObject = JObject.Parse(jsonContent);
        var entityType = jsonObject["entityType"]?.ToString();

        if (string.IsNullOrWhiteSpace(entityType) || !string.Equals(entityType, typeof(T).Name, StringComparison.OrdinalIgnoreCase))
        {
            var msg = $"Invalid or missing entityType. Expected '{typeof(T).Name}'";
            logger.LogWarning("BulkImportService: ExtractJsonDataAsync - {Message}", msg);
            throw new UserFriendlyException(msg, "400");
        }

        var items = jsonObject["items"]?.ToObject<List<T>>();

        if (items == null)
        {
            logger.LogError("BulkImportService: ExtractJsonDataAsync - Failed to parse JSON file");
            throw new UserFriendlyException("Invalid JSON format. Expected structure: { \"items\": [...] }", "400");
        }

        logger.LogInformation("BulkImportService: ExtractJsonDataAsync - Parsed JSON with {Count} records", items.Count);
        return items;
    }

}
