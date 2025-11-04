using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> e)
    {
        e.ToTable("Categories");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id)
            .HasColumnName("Id");

        e.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired()
            .IsUnicode();
        
        e.HasMany(x => x.ItemGroups)
            .WithOne(x=>x.Category)
            .HasForeignKey(x=>x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(true);


    }
}     