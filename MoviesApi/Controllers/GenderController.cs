using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/genders")]
public class GenderController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GenderController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<GenderDto>>> Get()
    {
        var entities = await _context.Genders.ToListAsync();
        var dto = _mapper.Map<List<GenderDto>>(entities);
        return dto;
    }

    [HttpGet("{id:int}", Name = "getGender")]
    public async Task<ActionResult<GenderDto>> Get(int id)
    {
        var entity = await _context.Genders.FirstOrDefaultAsync(gender => gender.Id == id);
        if (entity == null) return NotFound();

        var dto = _mapper.Map<GenderDto>(entity);
        return dto;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateGenderDto createGenderDto)
    {
        var entity = _mapper.Map<Gender>(createGenderDto);
        _context.Add((object)entity);
        await _context.SaveChangesAsync();
        var genderDto = _mapper.Map<GenderDto>(entity);
        return new CreatedAtRouteResult("getGender", new { id = genderDto.Id }, genderDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] CreateGenderDto createGenderDto)
    {
        var entity = _mapper.Map<Gender>(createGenderDto);
        entity.Id = id;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existGender = await _context.Genders.AnyAsync(gender => gender.Id == id);
        if (!existGender) return NotFound();

        _context.Remove(new Gender { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}