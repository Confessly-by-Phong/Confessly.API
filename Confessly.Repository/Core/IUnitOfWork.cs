using Confessly.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Repository.Core
{
    public interface IUnitOfWork : IDisposable
    {
        #region Repositories
        IRepository<User> Users { get; }
        #endregion

        Task<int> SaveChanges(CancellationToken cancellationToken = default);
    }
}
