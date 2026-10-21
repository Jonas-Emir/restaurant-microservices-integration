using RestauranteService.Models;

namespace RestauranteService.Data;

public interface IRestauranteRepository
{
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Restaurante>> GetAllRestaurantesAsync(CancellationToken cancellationToken = default);
    Task<Restaurante?> GetRestauranteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateRestauranteAsync(Restaurante restaurante, CancellationToken cancellationToken = default);
}
