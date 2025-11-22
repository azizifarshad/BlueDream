using BlueDream.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlueDream.Data.EntityConfigs
{
    public class CalendarConfiguration : IEntityTypeConfiguration<Calendar>
    {
        public void Configure(EntityTypeBuilder<Calendar> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.Date)
                .IsRequired();

            builder.Property(c => c.StartTime)
                .IsRequired();

            builder.Property(c => c.EndTime)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            // ایندکس روی تاریخ و زمان برای جستجوی سریع رزروها
            builder.HasIndex(c => new { c.Date, c.StartTime, c.EndTime });
        }
    }
}