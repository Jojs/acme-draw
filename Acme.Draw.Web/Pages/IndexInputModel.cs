using System.ComponentModel.DataAnnotations;

namespace Acme.Draw.Web.Pages;

public sealed class IndexInputModel
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required, EmailAddress]
    public string? Email { get; set; }

    [Required, DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Required]
    public string? SerialNumber { get; set; }
}