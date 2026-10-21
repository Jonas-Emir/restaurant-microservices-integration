using System.Reflection;
using ItemService.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Item> Itens => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}