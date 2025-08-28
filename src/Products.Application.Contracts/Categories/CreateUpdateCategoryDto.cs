using System;

namespace Products.Categories;

public class CreateUpdateCategoryDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; } = true;
}
