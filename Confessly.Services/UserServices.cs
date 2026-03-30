using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;
using Confessly.Services.Core;

namespace Confessly.Services
{
    public class UserServices : BaseServices, IUserServices
    {
        public UserServices(IUnitOfWork work, ILoggingService logger) : base(work, logger)
        {
        }
    }
}
