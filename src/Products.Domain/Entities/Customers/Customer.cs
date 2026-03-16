using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Products.Entities.Customers;

public class Customer : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PreferredLanguage { get; set; }
}
