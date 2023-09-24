using Application.Common.Models;

namespace Application.Common.Mappings;

public static class MappingExtensions
{
    public static PaginatedList<TDestination> PaginatedList<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
        => Models.PaginatedList<TDestination>.Create(queryable, pageNumber, pageSize);
}
