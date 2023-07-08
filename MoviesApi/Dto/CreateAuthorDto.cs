using System.ComponentModel.DataAnnotations;
using MoviesApi.Validations;

namespace MoviesApi.Dto;

public class CreateAuthorDto
{
    [Required] [StringLength(120)] public string Name { get; set; }
    public DateTime BirthdayDate { get; set; }

    [SizeFileValidation(4)]
    [TypeFileValidation(GroupTypeFile.Image)]
    public IFormFile Photo { get; set; }
}