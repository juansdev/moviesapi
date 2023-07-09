namespace MoviesApi.Entities;

public class MoviesAuthors
{
    public int AuthorId { get; set; }
    public int MovieId { get; set; }
    public string Character { get; set; }
    public int Order { get; set; }
    public Author Author { get; set; }
    public Movie Movie { get; set; }
}