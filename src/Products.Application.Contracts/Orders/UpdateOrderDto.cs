using System;

namespace Products.Orders;

public class UpdateOrderDto
{
    public string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
}
