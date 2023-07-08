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
        CreateMap<CreateMovieDto, Movie>().ForMember(author => author.Poster, options => options.Ignore());
        CreateMap<MoviePatchDto, Movie>().ReverseMap();
    }
}