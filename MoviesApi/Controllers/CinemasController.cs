using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dto;
using MoviesApi.Entities;
using NetTopologySuite.Geometries;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/cinemas")]
public class CinemasController : CustomBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly GeometryFactory _geometryFactory;
    private readonly IMapper _mapper;

    public CinemasController(ApplicationDbContext context, IMapper mapper, GeometryFactory geometryFactory) : base(
        context, mapper)
    {
        _context = context;
        _mapper = mapper;
        _geometryFactory = geometryFactory;
    }

    [HttpGet]
    public async Task<ActionResult<List<CinemaDto>>> Get()
    {
        return await Get<Cinema, CinemaDto>();
    }

    [HttpGet("{id:int}", Name = "getCinema")]
    public async Task<ActionResult<CinemaDto>> Get(int id)
    {
        return await Get<Cinema, CinemaDto>(id);
    }

    [HttpGet("nears")]
    public async Task<ActionResult<List<CinemaNearDto>>> Nears([FromQuery] CinemaNearFilterDto filter)
    {
        var locateUser = _geometryFactory.CreatePoint(new Coordinate(filter.Longitude, filter.Latitude));
        var cinema = await _context.Cinema
            .OrderBy(cinema => cinema.Location.Distance(locateUser))
            .Where(cinema => cinema.Location.IsWithinDistance(locateUser, filter.distanceByKms * 1000))
            .Select(cinema => new CinemaNearDto
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Latitude = cinema.Location.Y,
                Longitude = cinema.Location.X,
                DistanceByMeters = Math.Round(cinema.Location.Distance(locateUser))
            })
            .ToListAsync();
        return cinema;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateCinemaDto createCinemaDto)
    {
        return await Post<CreateCinemaDto, Cinema, CinemaDto>(createCinemaDto, "getCinema");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromBody] CreateCinemaDto createCinemaDto)
    {
        return await Put<CreateCinemaDto, Cinema>(id, createCinemaDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        return await Delete<Cinema>(id);
    }
}