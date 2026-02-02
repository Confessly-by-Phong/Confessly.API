using Confessly.Domain;
using Confessly.Domain.Core;
using Confessly.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Confessly.Repository.Core
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        #region Properties
        protected ConfesslyDbContext Context { get; }
        protected DbSet<TEntity> Table { get; }
        protected ILoggingService Logger { get; }
        protected string EntityName { get; }
        #endregion

        public BaseRepository(ConfesslyDbContext dbContext, ILoggingService logger)
        {
            Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Table = dbContext.Set<TEntity>();
            EntityName = typeof(TEntity).Name;
        }

        #region Methods
        /// <inheritdoc />
        public abstract Task Delete(TEntity entity,
            bool isSoftDeleted = true,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task Delete(Expression<Func<TEntity, bool>> predicate,
            bool isSoftDeleted = true,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<TEntity?> Get(Expression<Func<TEntity, bool>> predicate,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<TEntity?> Get(Guid id,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<IEnumerable<TEntity>> Get(Func<IQueryable<TEntity>, IQueryable<TEntity>> func,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<IEnumerable<TEntity>> Get(bool includeDeleted = false,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<TEntity> Insert(TEntity entity,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<IEnumerable<TEntity>> Insert(IList<TEntity> entities,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract Task<TEntity> Update(TEntity entity);
        #endregion
    }
}
