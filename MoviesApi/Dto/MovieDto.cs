namespace MoviesApi.Dto;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool InTheaters { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Poster { get; set; }
}