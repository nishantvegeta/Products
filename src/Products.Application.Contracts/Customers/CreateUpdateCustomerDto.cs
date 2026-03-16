using System;

namespace Products.Customers;

public class CreateUpdateCustomerDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string? PreferredLanguage { get; set; }
}
