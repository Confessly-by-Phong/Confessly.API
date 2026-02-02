using Confessly.Contracts.Authentication;
using Confessly.Domain.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Domain
{
    public class ConfesslyDbContext : DbContext
    {
        private readonly IUserContext _userContext;

        public ConfesslyDbContext(DbContextOptions options,
            IUserContext userContext) : base(options)
        {
            _userContext = userContext;
        }

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
            Guid userId = _userContext.GetCurrentUserId();
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
