using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OfficeOpenXml;
using Products.ExportServices;
using Volo.Abp.DependencyInjection;

namespace Products.AppServices.ExcelServices;

public class ExcelService : IExportService, ITransientDependency
{
    public async Task<byte[]> ExportAsync<TData>(List<TData> data, string fileType)
    {
        fileType = fileType?.ToLowerInvariant() ?? FileTypes.JsonType; // Default to JSON if null

        return fileType switch
        {
            FileTypes.CsvType => await Task.FromResult(ExportToCsv(data)),
            FileTypes.XlsxType => await Task.FromResult(ExportToExcel(data)),
            _ => await Task.FromResult(ExportToJson(data)), // Default JSON
        };
    }

    private byte[] ExportToJson<T>(List<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return Encoding.UTF8.GetBytes(json);
    }

    private byte[] ExportToCsv<T>(List<T> data)
    {
        var properties = typeof(T).GetProperties();
        var sb = new StringBuilder();

        // Header row
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        // Data rows
        foreach (var item in data)
        {
            var values = properties.Select(p =>
                p.GetValue(item, null)?.ToString()?.Replace(",", " ") ?? string.Empty);
            sb.AppendLine(string.Join(",", values));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private byte[] ExportToExcel<T>(List<T> data)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Export");

        var properties = typeof(T).GetProperties();

        // Header
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cells[1, i + 1].Value = properties[i].Name;
        }

        // Data
        int row = 2;
        foreach (var item in data)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[row, i + 1].Value = properties[i].GetValue(item);
            }
            row++;
        }

        return package.GetAsByteArray();
    }
}
