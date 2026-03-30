using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;

namespace Confessly.Services.Core
{
    public abstract class BaseServices(IUnitOfWork work, ILoggingService logger)
    {
        protected IUnitOfWork _work = work;
        protected ILoggingService _logger = logger;
    }
}
