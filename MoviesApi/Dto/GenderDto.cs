using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class GenderDto
{
    public int Id { get; set; }
    [Required] [StringLength(40)] public string Name { get; set; }
}