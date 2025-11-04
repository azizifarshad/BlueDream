using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs;

public class ItemGroupConfig : IEntityTypeConfiguration<ItemGroup>
{
    public void Configure(EntityTypeBuilder<ItemGroup> e)
    {
        e.ToTable("ItemGroup");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id)
            .HasColumnName("Id");

        e.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired(true)
            .IsUnicode(true);

        e.Property(x => x.Description)
            .HasColumnName("Description")
            .IsUnicode(true);

        e.Property(x => x.CategoryId)
            .HasColumnName("CategoryId")
            .IsRequired(true);

        e.HasMany(x => x.Items)
            .WithOne(x => x.ItemGroup)
            .HasForeignKey(x => x.ItemGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}