using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dto;
using MoviesApi.Entities;
using MoviesApi.Services;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly string _container = "movies";
    private readonly ApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IMapper _mapper;

    public MoviesController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
    {
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieDto>>> Get()
    {
        var movies = await _context.Movies.ToListAsync();
        return _mapper.Map<List<MovieDto>>(movies);
    }

    [HttpGet("{id}", Name = "getMovie")]
    public async Task<ActionResult<MovieDto>> Get(int id)
    {
        var movie = await _context.Movies.FirstOrDefaultAsync(author => author.Id == id);
        if (movie == null) return NotFound();

        return _mapper.Map<MovieDto>(movie);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] CreateMovieDto createMovieDto)
    {
        var movie = _mapper.Map<Movie>(createMovieDto);
        if (createMovieDto.Poster != null)
            using (var memoryStream = new MemoryStream())
            {
                await createMovieDto.Poster.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(createMovieDto.Poster.FileName);
                movie.Poster =
                    await _fileStorage.SaveFile(content, extension, _container, createMovieDto.Poster.ContentType);
            }

        _context.Add((object)movie);
        await _context.SaveChangesAsync();
        var dto = _mapper.Map<MovieDto>(movie);
        return new CreatedAtRouteResult("getMovie", new { id = movie.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromForm] CreateMovieDto createMovieDto)
    {
        var movie = await _context.Movies.FirstOrDefaultAsync(movie => movie.Id == id);
        if (movie == null) return NotFound();

        movie = _mapper.Map(createMovieDto, movie);
        if (createMovieDto.Poster != null)
            using (var memoryStream = new MemoryStream())
            {
                await createMovieDto.Poster.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(createMovieDto.Poster.FileName);
                movie.Poster =
                    await _fileStorage.EditFile(content, extension, _container, movie.Poster,
                        createMovieDto.Poster.ContentType);
            }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDto> patchDocument)
    {
        if (patchDocument == null) return BadRequest();

        var entity = await _context.Movies.FirstOrDefaultAsync(author => author.Id == id);
        if (entity == null) return NotFound();

        var entityDto = _mapper.Map<MoviePatchDto>(entity);
        patchDocument.ApplyTo(entityDto, ModelState);
        var isValid = TryValidateModel(entityDto);
        if (!isValid) return BadRequest(ModelState);

        _mapper.Map(entityDto, entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existMovie = await _context.Movies.AnyAsync(author => author.Id == id);
        if (!existMovie) return NotFound();

        _context.Remove(new Movie { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}