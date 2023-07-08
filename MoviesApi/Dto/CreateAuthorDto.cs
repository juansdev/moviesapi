using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class CreateAuthorDto
{
    [Required] [StringLength(120)] public string Name { get; set; }
    public DateTime BirthdayDate { get; set; }
}