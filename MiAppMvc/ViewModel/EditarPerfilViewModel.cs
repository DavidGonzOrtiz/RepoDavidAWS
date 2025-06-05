using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class EditarPerfilViewModel
{
    [Required]
    [EmailAddress]
    public string? UserName { get; set; }

    [Required]
    [MaxLength(10)]
    public string? FirstName { get; set; }

    [Required]
    [MaxLength(20)]
    public string? SecondName { get; set; }

}