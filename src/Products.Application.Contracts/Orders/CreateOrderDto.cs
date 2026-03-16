using System;

namespace Products.Orders;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
