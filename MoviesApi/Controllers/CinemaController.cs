using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/cinemas")]
public class CinemaController : CustomBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CinemaController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
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