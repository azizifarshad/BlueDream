using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs
{
    public class ItemConfig : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> e)
        {
            e.ToTable("Items");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
                .HasMaxLength(255)
                .IsUnicode(true)
                .IsRequired();

            e.Property(x => x.Price)
                .IsRequired();

            e.Property(x => x.Description)
                .IsUnicode(true)
                .IsRequired(false);

            e.Property(x => x.IsActive)
                .HasColumnType("bit")
                .IsRequired()
                .HasDefaultValue(true);

            e.Property(x => x.TimeSpend)
                .IsRequired();

            e.Property(x => x.Discount)
                .IsRequired(false);

            e.Property(x => x.ItemGroupId)
                .IsRequired()
                .HasDefaultValue(0);

            e.HasOne(x => x.ItemGroup)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ItemGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // ارتباط با CartItem
            e.HasMany(x => x.CartItems)
                .WithOne(ci => ci.Item)
                .HasForeignKey(ci => ci.ItemId);
        }
    }
}