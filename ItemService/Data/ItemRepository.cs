using ItemService.Dtos;
using ItemService.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemService.Data;

public class ItemRepository(AppDbContext context) : IItemRepository
{
    public async Task CreateItemAsync(int restauranteId, Item item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        item.AssociarAoRestaurante(restauranteId);
        await context.Itens.AddAsync(item, cancellationToken);
    }

    public async Task CreateRestauranteAsync(Restaurante restaurante, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(restaurante);
        await context.Restaurantes.AddAsync(restaurante, cancellationToken);
    }

    public async Task<bool> ExisteRestauranteExternoAsync(int restauranteIdExterno, CancellationToken cancellationToken = default)
    {
        return await context.Restaurantes
            .AnyAsync(r => r.IdExterno == restauranteIdExterno, cancellationToken);
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

        if (totalItems == 0)
        {
            return new PagedResult<Restaurante>([], paginationParams.PageNumber, paginationParams.PageSize, 0);
        }

        var items = await query
            .OrderBy(r => r.Id)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Restaurante>(items, paginationParams.PageNumber, paginationParams.PageSize, totalItems);
    }

    public async Task<Item?> GetItemAsync(int restauranteId, int itemId, CancellationToken cancellationToken = default)
    {
        return await context.Itens
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.IdRestaurante == restauranteId && i.Id == itemId, cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetItensDeRestauranteAsync(int restauranteId, CancellationToken cancellationToken = default)
    {
        return await context.Itens
            .AsNoTracking()
            .Where(i => i.IdRestaurante == restauranteId)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Item>> GetItensDeRestaurantePaginadosAsync(int restauranteId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParams);

        var query = context.Itens
            .AsNoTracking()
            .Where(i => i.IdRestaurante == restauranteId);

        var totalItems = await query.CountAsync(cancellationToken);

        if (totalItems == 0)
        {
            return new PagedResult<Item>([], paginationParams.PageNumber, paginationParams.PageSize, 0);
        }

        var items = await query
            .OrderBy(i => i.Id)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Item>(items, paginationParams.PageNumber, paginationParams.PageSize, totalItems);
    }

    public async Task<bool> RestauranteExisteAsync(int restauranteId, CancellationToken cancellationToken = default)
    {
        return await context.Restaurantes
            .AnyAsync(r => r.Id == restauranteId, cancellationToken);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) >= 0;
    }
}