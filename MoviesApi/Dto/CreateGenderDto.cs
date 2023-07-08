using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class CreateGenderDto
{
    [Required] [StringLength(40)] public string Name { get; set; }
}