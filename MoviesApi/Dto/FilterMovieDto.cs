namespace MoviesApi.Dto;

public class FilterMovieDto
{
    public int Page { get; set; } = 1;
    public int AmountRegisterByPage { get; set; } = 10;

    public PaginationDto Pagination => new() { Page = Page, AmountRegisterByPage = AmountRegisterByPage };
    public string Title { get; set; }
    public int GenderId { get; set; }
    public bool InTheaters { get; set; }
    public bool NextReleases { get; set; }
}