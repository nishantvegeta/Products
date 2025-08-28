using System;

namespace Products.Products;

public class CreateUpdateProductDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiryDate { get; set; }

    public Guid? CategoryId { get; set; }
}
