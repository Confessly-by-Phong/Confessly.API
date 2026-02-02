using Confessly.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Repository
{
    public partial class EntityRepository<TEntity> where TEntity : BaseEntity
    {
        #region Private Utilities
        protected IQueryable<TEntity> AddDeletedFilter(IQueryable<TEntity> query,
            in bool includeDeleted = false)
        {
            if (includeDeleted) return query;

            return query.Where(e => !e.IsDeleted);
        }
        #endregion
    }
}
