using ItemService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItemService.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.Nome)
            .IsRequired();

        builder.Property(i => i.Preco)
            .IsRequired();

        builder.Property(i => i.IdRestaurante)
            .IsRequired();

        builder.HasOne(i => i.Restaurante)
            .WithMany(r => r.Itens)
            .HasForeignKey(i => i.IdRestaurante);
    }
}
