using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Products.ExportServices;

public interface IExportService
{
    Task<byte[]> ExportAsync<TData>(List<TData> data, string fileType);
}
