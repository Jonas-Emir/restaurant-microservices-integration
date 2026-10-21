using Microsoft.EntityFrameworkCore;
using RestauranteService.Dtos;
using RestauranteService.Models;

namespace RestauranteService.Data;

public class RestauranteRepository(AppDbContext context) : IRestauranteRepository
{
    public async Task CreateRestauranteAsync(Restaurante restaurante, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(restaurante);
        await context.Restaurantes.AddAsync(restaurante, cancellationToken);
    }

    public async Task<IEnumerable<Restaurante>> GetAllRestaurantesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Restaurantes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Restaurante>> GetRestaurantesPaginadosAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParams);

        var query = context.Restaurantes.AsNoTracking();
        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.Id)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Restaurante>(items, paginationParams.PageNumber, paginationParams.PageSize, totalItems);
    }

    public async Task<Restaurante?> GetRestauranteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Restaurantes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) >= 0;
    }
}