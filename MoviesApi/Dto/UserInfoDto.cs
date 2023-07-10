using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class UserInfoDto
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
}