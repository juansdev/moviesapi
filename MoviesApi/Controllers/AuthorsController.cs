using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dto;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using MoviesApi.Services;

namespace MoviesApi.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly string _container = "authors";
    private readonly ApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IMapper _mapper;

    public AuthorsController(ApplicationDbContext context, IMapper mapper, IFileStorage fileStorage)
    {
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> Get([FromQuery] PaginationDto paginationDto)
    {
        var queryable = _context.Authors.AsQueryable();
        await HttpContext.AddParametersPagination(queryable, paginationDto.AmountRegisterByPage);
        var entities = await queryable.Paginate(paginationDto).ToListAsync();
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
    public async Task<ActionResult> Post([FromForm] CreateAuthorDto createAuthorDto)
    {
        var entity = _mapper.Map<Author>(createAuthorDto);
        if (createAuthorDto.Photo != null)
            using (var memoryStream = new MemoryStream())
            {
                await createAuthorDto.Photo.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(createAuthorDto.Photo.FileName);
                entity.Photo =
                    await _fileStorage.SaveFile(content, extension, _container, createAuthorDto.Photo.ContentType);
            }

        _context.Add((object)entity);
        await _context.SaveChangesAsync();
        var dto = _mapper.Map<AuthorDto>(entity);
        return new CreatedAtRouteResult("getAuthor", new { id = entity.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromForm] CreateAuthorDto createAuthorDto)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(author => author.Id == id);
        if (author == null) return NotFound();

        author = _mapper.Map(createAuthorDto, author);
        if (createAuthorDto.Photo != null)
            using (var memoryStream = new MemoryStream())
            {
                await createAuthorDto.Photo.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                var extension = Path.GetExtension(createAuthorDto.Photo.FileName);
                author.Photo =
                    await _fileStorage.EditFile(content, extension, _container, author.Photo,
                        createAuthorDto.Photo.ContentType);
            }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<AuthorPatchDto> patchDocument)
    {
        if (patchDocument == null) return BadRequest();

        var entity = await _context.Authors.FirstOrDefaultAsync(author => author.Id == id);
        if (entity == null) return NotFound();

        var entityDto = _mapper.Map<AuthorPatchDto>(entity);
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
        var existAuthor = await _context.Authors.AnyAsync(author => author.Id == id);
        if (!existAuthor) return NotFound();

        _context.Remove(new Author { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}