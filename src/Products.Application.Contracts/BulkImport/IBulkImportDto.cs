using System;

namespace Products.BulkImport;

public interface IBulkImportDto
{
    public string DataIdentifier { get; set; }
}
