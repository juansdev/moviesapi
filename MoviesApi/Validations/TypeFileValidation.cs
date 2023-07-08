using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Validations;

public class TypeFileValidation : ValidationAttribute
{
    private readonly string[] _validTypes;

    public TypeFileValidation(string[] validTypes)
    {
        _validTypes = validTypes;
    }

    public TypeFileValidation(GroupTypeFile groupTypeFile)
    {
        if (groupTypeFile == GroupTypeFile.Image)
            _validTypes = new string[3] { "image/jpeg", "image/png", "image/gif" };
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var formFile = value as IFormFile;
        if (formFile == null) return ValidationResult.Success;

        if (!_validTypes.Contains(formFile.ContentType))
            return new ValidationResult(
                $"El tipo del archivo debe ser uno de los siguientes: {string.Join(",", _validTypes)}");

        return ValidationResult.Success;
    }
}