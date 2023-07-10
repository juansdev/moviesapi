using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class CreateReviewDto
{
    public string Comment { get; set; }
    [Range(1, 5)] public int Score { get; set; }
}