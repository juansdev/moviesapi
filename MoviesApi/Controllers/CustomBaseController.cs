using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dto;
using MoviesApi.Entities;
using MoviesApi.Helpers;

namespace MoviesApi.Controllers;

public class CustomBaseController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CustomBaseController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    protected async Task<List<TDto>> Get<TEntity, TDto>() where TEntity : class
    {
        var entities = await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        var dto = _mapper.Map<List<TDto>>(entities);
        return dto;
    }

    protected async Task<List<TDto>> Get<TEntity, TDto>(PaginationDto paginationDto) where TEntity : class
    {
        var queryable = _context.Set<TEntity>().AsQueryable();
        return await Get<TEntity, TDto>(paginationDto, queryable);
    }

    protected async Task<List<TDto>> Get<TEntity, TDto>(PaginationDto paginationDto, IQueryable<TEntity> queryable)
        where TEntity : class
    {
        await HttpContext.AddParametersPagination(queryable, paginationDto.AmountRegisterByPage);
        var entities = await queryable.Paginate(paginationDto).ToListAsync();
        return _mapper.Map<List<TDto>>(entities);
    }

    protected async Task<ActionResult<TDto>> Get<TEntity, TDto>(int id) where TEntity : class, IId
    {
        var entity = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        return _mapper.Map<TDto>(entity);
    }

    protected async Task<ActionResult> Post<TCreate, TEntity, TRead>(TCreate createDto, string pathName)
        where TEntity : class, IId
    {
        var entity = _mapper.Map<TEntity>(createDto);
        _context.Add((object)entity);
        await _context.SaveChangesAsync();
        var dtoRead = _mapper.Map<TRead>(entity);
        return new CreatedAtRouteResult(pathName, new { id = entity.Id }, dtoRead);
    }

    protected async Task<ActionResult> Put<TCreate, TEntity>(int id, TCreate createDto) where TEntity : class, IId
    {
        var entity = _mapper.Map<TEntity>(createDto);
        entity.Id = id;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    protected async Task<ActionResult> Patch<TEntity, TDto>(int id, JsonPatchDocument<TDto> patchDocument)
        where TDto : class where TEntity : class, IId
    {
        if (patchDocument == null) return BadRequest();

        var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(author => author.Id == id);
        if (entity == null) return NotFound();

        var entityDto = _mapper.Map<TDto>(entity);
        patchDocument.ApplyTo(entityDto, ModelState);
        var isValid = TryValidateModel(entityDto);
        if (!isValid) return BadRequest(ModelState);

        _mapper.Map(entityDto, entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    protected async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, IId, new()
    {
        var exist = await _context.Set<TEntity>().AnyAsync(x => x.Id == id);
        if (!exist) return NotFound();

        _context.Remove(new TEntity { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}