using MoviesApi.Dto;

namespace MoviesApi.Helpers;

public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDto paginationDto)
    {
        return queryable.Skip((paginationDto.Page - 1) * paginationDto.AmountRegisterByPage)
            .Take(paginationDto.AmountRegisterByPage);
    }
}