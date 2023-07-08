using MoviesApi.Validations;

namespace MoviesApi.Dto;

public class CreateMovieDto : MoviePatchDto
{
    [SizeFileValidation(4)]
    [TypeFileValidation(GroupTypeFile.Image)]
    public IFormFile Poster { get; set; }
}