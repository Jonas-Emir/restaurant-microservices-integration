using ItemService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItemService.Data.Configurations;

public class RestauranteConfiguration : IEntityTypeConfiguration<Restaurante>
{
    public void Configure(EntityTypeBuilder<Restaurante> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.IdExterno)
            .IsRequired();

        builder.Property(r => r.Nome)
            .IsRequired();

        builder.Property(r => r.Endereco)
            .IsRequired();

        builder.Property(r => r.Site)
            .IsRequired();

        builder.HasMany(r => r.Itens)
            .WithOne(i => i.Restaurante!)
            .HasForeignKey(i => i.IdRestaurante)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
