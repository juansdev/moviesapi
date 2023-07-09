using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class CreateCinemaDto
{
    [Required] [StringLength(120)] public string Name { get; set; }
}