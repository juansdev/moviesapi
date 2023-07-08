using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Helpers;

public static class HttpContextExtensions
{
    public static async Task AddParametersPagination<T>(this HttpContext httpContext, IQueryable<T> queryable,
        int amountRegistersByPage)
    {
        double amount = await queryable.CountAsync();
        var amountPages = Math.Ceiling(amount / amountRegistersByPage);
        httpContext.Response.Headers.Add("amountPages", amountPages.ToString());
    }
}