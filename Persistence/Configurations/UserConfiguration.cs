using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
            .HasMaxLength(255);

        builder.Property(x => x.Salt)
            .HasMaxLength(32);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(64);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}