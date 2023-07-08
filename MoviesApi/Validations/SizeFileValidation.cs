using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Validations;

public class SizeFileValidation : ValidationAttribute
{
    private readonly int _sizeMaxByMb;

    public SizeFileValidation(int sizeMaxByMb)
    {
        _sizeMaxByMb = sizeMaxByMb;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var formFile = value as IFormFile;
        if (formFile == null) return ValidationResult.Success;

        if (formFile.Length > _sizeMaxByMb * 1024 * 1024)
            return new ValidationResult($"El peso del archivo no debe ser mayor a {_sizeMaxByMb}mb");

        return ValidationResult.Success;
    }
}