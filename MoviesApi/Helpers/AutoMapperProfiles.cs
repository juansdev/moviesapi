﻿using AutoMapper;
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
        CreateMap<CreateAuthorDto, Author>();
    }
}