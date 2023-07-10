using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dto;
using MoviesApi.Entities;
using MoviesApi.Helpers;

namespace MoviesApi.Controllers;

[Route("api/movies/{movieId:int}/reviews")]
[ServiceFilter(typeof(MovieExistAttribute))]
public class ReviewController : CustomBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReviewController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReviewDto>>> Get(int movieId, [FromQuery] PaginationDto paginationDto)
    {
        var queryable = _context.Reviews.Include(review => review.User).AsQueryable();
        queryable = queryable.Where(review => review.MovieId == movieId);
        return await Get<Review, ReviewDto>(paginationDto, queryable);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post(int movieId, [FromBody] CreateReviewDto createReviewDto)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(user => user.Type == ClaimTypes.NameIdentifier)?.Value;
        var existReview =
            await _context.Reviews.AnyAsync(review => review.MovieId == movieId && review.UserId == userId);
        if (existReview) return BadRequest("El usuario ya ha escrito un review de esta película");

        var review = _mapper.Map<Review>(createReviewDto);
        review.MovieId = movieId;
        review.UserId = userId;
        _context.Add((object)review);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{reviewId:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Put(int movieId, int reviewId, [FromBody] CreateReviewDto createReviewDto)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(review => review.Id == reviewId);
        if (review == null) return NotFound();

        var userId = HttpContext.User.Claims.FirstOrDefault(user => user.Type == ClaimTypes.NameIdentifier)?.Value;
        if (review.UserId != userId) return BadRequest("No tiene permisos de editar este review");

        review = _mapper.Map(createReviewDto, review);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{reviewId:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Delete(int reviewId)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(review => review.Id == reviewId);
        if (review == null) return NotFound();

        var userId = HttpContext.User.Claims.FirstOrDefault(user => user.Type == ClaimTypes.NameIdentifier)?.Value;
        if (review.UserId != userId) return Forbid();

        _context.Remove(review);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}