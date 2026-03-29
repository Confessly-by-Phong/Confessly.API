using Confessly.Contracts.Authentication;
using Confessly.Domain;
using Confessly.Domain.Core;
using Microsoft.EntityFrameworkCore;

namespace Confessly.Infrastructure
{
    public class ConfesslyDbContext : DbContext
    {
        private readonly IUserContext? _userContext;

        public ConfesslyDbContext(DbContextOptions options,
            IUserContext userContext) : base(options)
        {
            _userContext = userContext;
        }

        internal ConfesslyDbContext(DbContextOptions options) : base(options)
        {
            _userContext = null;
        }

        #region DbSets
        public DbSet<User> Users { get; set; }
        #endregion

        public override int SaveChanges()
        {
            SetAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        private void SetAuditFields()
        {
            Guid userId = _userContext?.GetCurrentUserId() ?? Guid.Empty;
            var now = DateTimeOffset.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = userId;
                        entry.Entity.CreatedTime = now;
                        entry.Entity.UpdatedBy = userId;
                        entry.Entity.UpdatedTime = now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = userId;
                        entry.Entity.UpdatedTime = now;
                        break;
                }
            }
        }

    }
}
