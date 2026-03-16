using System;

namespace Products.Customers;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
}
