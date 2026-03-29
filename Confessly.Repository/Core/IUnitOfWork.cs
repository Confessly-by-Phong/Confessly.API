using Confessly.Domain;

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
