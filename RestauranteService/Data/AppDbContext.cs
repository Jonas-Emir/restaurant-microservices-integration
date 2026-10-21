using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RestauranteService.Models;

namespace RestauranteService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
