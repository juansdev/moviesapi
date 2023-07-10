using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MoviesApi.Entities;

public class Review : IId
{
    public string Comment { get; set; }
    [Range(1, 5)] public int Score { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }
    public int Id { get; set; }
}