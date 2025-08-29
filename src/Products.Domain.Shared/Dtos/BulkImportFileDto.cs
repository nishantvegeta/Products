using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Products.Dtos;

public class BulkImportFileDto
{
    [Required]
    public IFormFile File { get; set; }
}
