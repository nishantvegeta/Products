using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Products.BulkImport;

public interface IBulkImportService
{
    Task<List<T>> GetDataAsync<T>(MemoryStream stream, string extension) where T : IBulkImportDto;
    Task<List<T>> GetDataAsync<T>(IFormFile file, string tableIdentifier) where T : IBulkImportDto;
    Task<T> GetJsonDataAsync<T>(IFormFile file) where T : class;
}
