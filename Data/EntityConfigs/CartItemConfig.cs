using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs
{
    public class CartItemConfig : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> e)
        {
            e.ToTable("CartItems");
            e.HasKey(x => x.Id);

            e.Property(x => x.CartId)
                .IsRequired();

            e.Property(x => x.ItemId)
                .IsRequired();

            e.Property(x => x.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            e.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            e.Property(x => x.DiscountPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            e.HasOne(x => x.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Item)
                .WithMany(i => i.CartItems)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}