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
        e.Property(x => x.Id)
            .HasColumnName("Id");
        
        e.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(255)
            .IsUnicode(true)
            .IsRequired(true);
        
        e.Property(x => x.Price)
            .HasColumnName("Price")
            .IsRequired();

        e.Property(x => x.Description)
            .HasColumnName("Description")
            .IsRequired(false)
            .IsUnicode(true);
        
        e.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired()
            .HasDefaultValue(true);
        
        e.Property(x => x.TimeSpend)
            .HasColumnName("TimeSpend")
            .IsRequired();

        e.Property(x => x.Discount)
            .HasColumnName("Discount")
            .IsRequired(false);
            //????
            //.HasPrecision(0, 100);

        e.Property(x => x.ItemGroupId)
            .HasColumnName("ItemGroupId")
            .IsRequired(true)
            .HasDefaultValue(0);
        
        
        e.HasMany(x => x.Carts)
            .WithMany(x => x.Items);

        e.HasOne(x => x.ItemGroup)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ItemGroupId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(true);
    }
}