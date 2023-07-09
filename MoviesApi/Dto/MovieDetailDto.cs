namespace MoviesApi.Dto;

public class MovieDetailDto : MovieDto
{
    public List<GenderDto> Genders { get; set; }
    public List<AuthorMovieDetailDto> Authors { get; set; }
}