using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Products.Entities.Categories;

public class Category : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; } = true;
}
