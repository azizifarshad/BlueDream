using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder< ApplicationUser> e)
    {
        e.ToTable("ApplicationUsers");
        e.HasKey(x => x.Id);
        e.Property(x => x.Id)
            .HasColumnName("Id");

        e.Property(x => x.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired(true);

        e.Property(x => x.Gender)
            .HasColumnName("Gender")
            .IsRequired(true);

        e.HasMany(x => x.Carts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);


    }

}