using MoviesApi.Validations;

namespace MoviesApi.Dto;

public class CreateAuthorDto : AuthorPatchDto
{
    [SizeFileValidation(4)]
    [TypeFileValidation(GroupTypeFile.Image)]
    public IFormFile Photo { get; set; }
}