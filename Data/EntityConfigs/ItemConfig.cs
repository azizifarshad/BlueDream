using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs;

public class ItemConfig : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> e)
    {
        e.ToTable("Items");

        e.HasKey(x => x.Id);

        e.Property(x => x.Name)
            .HasMaxLength(255)
            .IsUnicode()
            .IsRequired();

        e.Property(x => x.Price)
            .IsRequired();

        e.Property(x => x.Description)
            .IsUnicode();

        e.Property(x => x.IsActive)
            .HasDefaultValue(true);

        e.Property(x => x.TimeSpend)
            .IsRequired();

        e.Property(x => x.Discount)
            .HasDefaultValue(0);

        e.Property(x => x.ItemGroupId)
            .IsRequired();

        e.HasOne(x => x.ItemGroup)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ItemGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Many-to-Many با کانفیگ استاندارد
        e.HasMany(x => x.Carts)
            .WithMany(x => x.Items)
            .UsingEntity(j =>
            {
                j.ToTable("CartItems");
                j.HasKey("ItemsId", "CartsId");
            });
    }
}