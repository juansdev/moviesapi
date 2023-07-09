using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities;

public class Cinema : IId
{
    [Required] [StringLength(120)] public string Name { get; set; }

    public List<MoviesCinemas> MoviesCinemas { get; set; }
    public int Id { get; set; }
}