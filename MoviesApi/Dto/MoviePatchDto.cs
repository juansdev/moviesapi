using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class MoviePatchDto
{
    [Required] [StringLength(300)] public string Title { get; set; }
    public bool InTheaters { get; set; }
    public DateTime ReleaseDate { get; set; }
}