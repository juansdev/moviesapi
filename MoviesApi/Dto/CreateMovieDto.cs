using Microsoft.AspNetCore.Mvc;
using MoviesApi.Helpers;
using MoviesApi.Validations;

namespace MoviesApi.Dto;

public class CreateMovieDto : MoviePatchDto
{
    [SizeFileValidation(4)]
    [TypeFileValidation(GroupTypeFile.Image)]
    public IFormFile Poster { get; set; }

    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int> GendersIds { get; set; }

    [ModelBinder(BinderType = typeof(TypeBinder<List<CreateAuthorMovieDto>>))]
    public List<CreateAuthorMovieDto> Authors { get; set; }
}