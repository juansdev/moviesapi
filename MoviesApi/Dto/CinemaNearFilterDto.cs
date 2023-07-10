using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dto;

public class CinemaNearFilterDto
{
    public int distanceByKms = 10;
    [Range(-90, 90)] public double Latitude { get; set; }
    [Range(-180, 180)] public double Longitude { get; set; }
    public int distanceMaxKms { get; set; }

    public int DistanceByKms
    {
        get => distanceByKms;
        set => distanceByKms = value > distanceMaxKms ? distanceMaxKms : value;
    }
}