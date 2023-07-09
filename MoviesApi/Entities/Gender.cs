using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities;

public class Gender : IId
{
    [Required] [StringLength(40)] public string Name { get; set; }
    public List<MoviesGenders> MoviesGenders { get; set; }
    public int Id { get; set; }
}