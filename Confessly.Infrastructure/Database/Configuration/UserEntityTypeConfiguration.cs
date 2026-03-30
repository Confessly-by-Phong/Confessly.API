using Confessly.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Confessly.Infrastructure.Database.Configuration
{
    internal class UserEntityTypeConfiguration : BaseEntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(u => u.Username).IsRequired();
            builder.Property(u => u.Password).IsRequired();

            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
