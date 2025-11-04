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
        e.Property(x => x.Id)
            .HasColumnName("Id");
        
        e.Property(x => x.TimeStart)
            .HasColumnName("TimeStart")
            .IsRequired(true)
            .HasColumnType("datetime2(0)");
        
        e.Property(x => x.TotalTime)
            .HasColumnName("TotalTime")
            .IsRequired(true);

        e.Property(x => x.PriceWithoutCount)
            .HasColumnName("PriceWithoutCount")
            .IsRequired(true);

        e.Property(x => x.FinalPrice)
            .HasColumnName("FinalPrice")
            .IsRequired(true);

        e.Property(x => x.DiscountPrice)
            .HasColumnName("DiscountPrice")
            .IsRequired(true);
        
        e.Property(x => x.Status)
            .HasColumnName("Status")
            .IsRequired(true)
            .HasDefaultValue(1);
        
        e.Property(x => x.UserId)
            .HasColumnName("UserId")
            .IsRequired(true);
        
        e.HasOne(x => x.User)
            .WithMany(x=>x.Carts)
            .HasForeignKey(x => x.UserId);
        
        e.HasMany(x => x.Items)
            .WithMany(x => x.Carts);
        
       }
}