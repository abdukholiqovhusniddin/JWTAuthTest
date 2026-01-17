using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Username).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(255);
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.Property(e => e.Role).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasQueryFilter(e => e.IsActive);


        builder.HasData(
            new User
            {
                Id = new Guid("1cd5ed74-9327-4446-9c76-adfc28d3f5bd"),
                Username = "admin",
                Email = "abdukholiqovh@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = UserRole.Admin
            });
           
    }
}
