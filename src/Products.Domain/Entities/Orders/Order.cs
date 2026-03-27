using System;
using Products.Entities.Customers;
using Volo.Abp.Domain.Entities.Auditing;

namespace Products.Entities.Orders;

public class Order : FullAuditedAggregateRoot<Guid>
{
    public Guid CustomerId { get; set; }
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = "pending";
    public virtual Customer Customer { get; set; }
}
