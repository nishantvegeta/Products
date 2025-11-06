using System;

namespace Products.Categories;

public class CategoryFilter
{
    public string? SearchKeyword { get; set; }
    public bool? IsActive { get; set; }
    public string? ExportFormat { get; set; }
}
