using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities;

public class Movie : IId
{
    [Required] [StringLength(300)] public string Title { get; set; }
    public bool InTheaters { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Poster { get; set; }
    public List<MoviesAuthors> MoviesAuthors { get; set; }
    public List<MoviesGenders> MoviesGenders { get; set; }
    public int Id { get; set; }
}