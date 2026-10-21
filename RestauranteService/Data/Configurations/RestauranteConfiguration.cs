using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteService.Models;

namespace RestauranteService.Data.Configurations;

public class RestauranteConfiguration : IEntityTypeConfiguration<Restaurante>
{
    public void Configure(EntityTypeBuilder<Restaurante> builder)
    {
        builder.ToTable("Restaurantes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Nome)
            .IsRequired();

        builder.Property(r => r.Endereco)
            .IsRequired();

        builder.Property(r => r.Site)
            .IsRequired();
    }
}
