using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs;

public class CartConfig : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> e)
    {
        e.ToTable("Carts");

        e.HasKey(x => x.Id);

        e.Property(x => x.TimeStart)
            .HasColumnType("datetime2(0)")
            .IsRequired();

        e.Property(x => x.TotalTime)
            .IsRequired();

        e.Property(x => x.PriceWithoutCount)
            .IsRequired();

        e.Property(x => x.FinalPrice)
            .IsRequired();

        e.Property(x => x.DiscountPrice)
            .IsRequired();

        e.Property(x => x.Status)
            .HasDefaultValue(1)
            .IsRequired();

        // ⬅ مهم: GuestId باید اضافه شود
        e.Property(x => x.GuestId)
            .HasMaxLength(100)
            .IsRequired(false);

        // ⬅ UserId دیگر اجباری نیست
        e.Property(x => x.UserId)
            .IsRequired(false);

        e.HasOne(x => x.User)
            .WithMany(x => x.Carts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // ⬅ کانفیگ درست Many-to-Many
        e.HasMany(x => x.Items)
            .WithMany(x => x.Carts)
            .UsingEntity(j =>
            {
                j.ToTable("CartItems");   // نام جدول واسط
                j.HasKey("CartsId", "ItemsId"); // کلید مرکب
            });
    }
}