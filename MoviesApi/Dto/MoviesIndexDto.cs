namespace MoviesApi.Dto;

public class MoviesIndexDto
{
    public List<MovieDto> FutureReleases { get; set; }
    public List<MovieDto> InTheaters { get; set; }
}