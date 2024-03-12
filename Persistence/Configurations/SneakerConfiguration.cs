using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SneakerConfiguration: IEntityTypeConfiguration<Sneaker>
{
    public void Configure(EntityTypeBuilder<Sneaker> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Brand)
            .HasMaxLength(100);

        builder.OwnsOne(x => x.Price, priceBuilder =>
        {
            priceBuilder.Property(p => p.Currency).HasConversion<string>();
        });

        builder.OwnsOne(x => x.Size, sizeBuilder =>
        {
            sizeBuilder.Property(s => s.Country).HasConversion<string>();
        });

        builder.Property(x => x.Rate).HasConversion(
            rate => rate.Value,
            value => new Rate(value));

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}