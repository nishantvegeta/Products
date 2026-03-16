using System;

namespace Products.Orders;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public DateTime CreationTime { get; set; }
}
