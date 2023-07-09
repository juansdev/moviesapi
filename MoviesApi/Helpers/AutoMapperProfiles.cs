using AutoMapper;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Gender, GenderDto>().ReverseMap();
        CreateMap<CreateGenderDto, Gender>();

        CreateMap<Author, AuthorDto>().ReverseMap();
        CreateMap<CreateAuthorDto, Author>().ForMember(author => author.Photo, options => options.Ignore());
        CreateMap<AuthorPatchDto, Author>().ReverseMap();

        CreateMap<Movie, MovieDto>().ReverseMap();
        CreateMap<CreateMovieDto, Movie>()
            .ForMember(author => author.Poster, options => options.Ignore())
            .ForMember(author => author.MoviesGenders, options => options.MapFrom(MapMoviesGenders))
            .ForMember(author => author.MoviesAuthors, options => options.MapFrom(MapMoviesAuthors));
        CreateMap<Movie, MovieDetailDto>()
            .ForMember(movieDetail => movieDetail.Genders, options => options.MapFrom(MapMoviesGenders))
            .ForMember(movieDetail => movieDetail.Authors, options => options.MapFrom(MapMoviesAuthors));
        CreateMap<MoviePatchDto, Movie>().ReverseMap();
    }

    private List<AuthorMovieDetailDto> MapMoviesAuthors(Movie movie, MovieDetailDto movieDetailDto)
    {
        var result = new List<AuthorMovieDetailDto>();
        if (movie.MoviesAuthors == null) return result;

        foreach (var movieAuthor in movie.MoviesAuthors)
            result.Add(new AuthorMovieDetailDto
            {
                AuthorId = movieAuthor.AuthorId,
                Character = movieAuthor.Character,
                NameCharacter = movieAuthor.Author.Name
            });

        return result;
    }

    private List<GenderDto> MapMoviesGenders(Movie movie, MovieDetailDto movieDetailDto)
    {
        var result = new List<GenderDto>();
        if (movie.MoviesGenders == null) return result;

        foreach (var movieGender in movie.MoviesGenders)
            result.Add(new GenderDto { Id = movieGender.GenderId, Name = movieGender.Gender.Name });

        return result;
    }

    private List<MoviesGenders> MapMoviesGenders(CreateMovieDto createMovieDto, Movie movie)
    {
        var result = new List<MoviesGenders>();
        if (createMovieDto.GendersIds == null) return result;

        foreach (var id in createMovieDto.GendersIds) result.Add(new MoviesGenders { GenderId = id });

        return result;
    }

    private List<MoviesAuthors> MapMoviesAuthors(CreateMovieDto createMovieDto, Movie movie)
    {
        var result = new List<MoviesAuthors>();
        if (createMovieDto.Authors == null) return result;

        foreach (var author in createMovieDto.Authors)
            result.Add(new MoviesAuthors { AuthorId = author.AuthorId, Character = author.Character });

        return result;
    }
}