using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoviesApi.Tests;

public class UserFalseFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.Email, "example@hotmail.com"),
            new(ClaimTypes.Name, "example@hotmail.com"),
            new(ClaimTypes.NameIdentifier, "9722b56a-77ea-4e41-941d-e319b6eb3712")
        }, "prueba"));

        await next();
    }
}