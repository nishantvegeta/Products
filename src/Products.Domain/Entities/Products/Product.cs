using System;
using Products.Entities.Categories;
using Volo.Abp.Domain.Entities.Auditing;

namespace Products.Entities.Products;

public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiryDate { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; }
}
