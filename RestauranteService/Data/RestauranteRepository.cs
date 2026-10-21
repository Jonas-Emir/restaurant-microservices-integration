using Microsoft.EntityFrameworkCore;
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