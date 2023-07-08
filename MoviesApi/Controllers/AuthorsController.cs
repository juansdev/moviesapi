using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dto;
using MoviesApi.Entities;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuthorsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> Get()
    {
        var entities = await _context.Authors.ToListAsync();
        return _mapper.Map<List<AuthorDto>>(entities);
    }

    [HttpGet("{id}", Name = "getAuthor")]
    public async Task<ActionResult<AuthorDto>> Get(int id)
    {
        var entity = await _context.Authors.FirstOrDefaultAsync(author => author.Id == id);
        if (entity == null) return NotFound();

        var dto = _mapper.Map<AuthorDto>(entity);
        return dto;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateAuthorDto createAuthorDto)
    {
        var entity = _mapper.Map<Author>(createAuthorDto);
        _context.Add((object)entity);
        await _context.SaveChangesAsync();
        var dto = _mapper.Map<AuthorDto>(entity);
        return new CreatedAtRouteResult("getAuthor", new { id = entity.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] CreateAuthorDto createAuthorDto)
    {
        var entity = _mapper.Map<Author>(createAuthorDto);
        entity.Id = id;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existAuthor = await _context.Authors.AnyAsync(author => author.Id == id);
        if (!existAuthor) return NotFound();

        _context.Remove(new Author { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}