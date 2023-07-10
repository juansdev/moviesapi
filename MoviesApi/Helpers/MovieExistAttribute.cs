using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Helpers;

public class MovieExistAttribute : Attribute, IAsyncResourceFilter
{
    private readonly ApplicationDbContext _dbContext;

    public MovieExistAttribute(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var movieIdObject = context.HttpContext.Request.RouteValues["movieId"];
        if (movieIdObject == null) return;

        var movieId = int.Parse(movieIdObject.ToString());
        var existMovie = await _dbContext.Movies.AnyAsync(movie => movie.Id == movieId);
        if (!existMovie)
            context.Result = new NotFoundResult();
        else
            await next();
    }
}