using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Repository.Core
{
    public interface IUnitOfWork : IDisposable
    {
        #region Repositories
        #endregion

        Task<int> SaveChanges(CancellationToken cancellationToken = default);
    }
}
