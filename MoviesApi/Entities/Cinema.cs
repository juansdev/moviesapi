using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace MoviesApi.Entities;

public class Cinema : IId
{
    [Required] [StringLength(120)] public string Name { get; set; }
    public Point Location { get; set; }

    public List<MoviesCinemas> MoviesCinemas { get; set; }
    public int Id { get; set; }
}