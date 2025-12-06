using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> e)
        {
            e.ToTable("Carts");
            e.HasKey(x => x.Id);

            e.Property(x => x.TimeStart)
                .IsRequired()
                .HasColumnType("datetime2(0)");

            e.Property(x => x.TotalTime)
                .IsRequired()
                .HasPrecision(18, 2);    

            e.Property(x => x.PriceWithoutCount)
                .IsRequired()
                .HasPrecision(18, 2);    

            e.Property(x => x.FinalPrice)
                .IsRequired()
                .HasPrecision(18, 2);   

            e.Property(x => x.DiscountPrice)
                .IsRequired()
                .HasPrecision(18, 2);   

            e.Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue(StatusEnum.Created);

            e.Property(x => x.UserId)
                .IsRequired();

            e.HasOne(x => x.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(x => x.UserId);

            e.HasMany(x => x.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);
        }
    }
}