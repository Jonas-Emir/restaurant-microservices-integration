using ItemService.Dtos;
using ItemService.Models;

namespace ItemService.Data;

public interface IItemRepository
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Restaurante>> GetAllRestaurantesAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<Restaurante>> GetRestaurantesPaginadosAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task CreateRestauranteAsync(Restaurante restaurante, CancellationToken cancellationToken = default);
    Task<bool> RestauranteExisteAsync(int restauranteId, CancellationToken cancellationToken = default);
    Task<bool> ExisteRestauranteExternoAsync(int restauranteIdExterno, CancellationToken cancellationToken = default);

    Task<IEnumerable<Item>> GetItensDeRestauranteAsync(int restauranteId, CancellationToken cancellationToken = default);
    Task<PagedResult<Item>> GetItensDeRestaurantePaginadosAsync(int restauranteId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<Item?> GetItemAsync(int restauranteId, int itemId, CancellationToken cancellationToken = default);
    Task CreateItemAsync(int restauranteId, Item item, CancellationToken cancellationToken = default);
}
