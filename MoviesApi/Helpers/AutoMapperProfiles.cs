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
        CreateMap<MoviePatchDto, Movie>().ReverseMap();
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