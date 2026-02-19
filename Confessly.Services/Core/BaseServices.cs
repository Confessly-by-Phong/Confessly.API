using Confessly.Logging.Interfaces;
using Confessly.Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Services.Core
{
    public abstract class BaseServices(IUnitOfWork work, ILoggingService logger)
    {
        protected IUnitOfWork _work = work;
        protected ILoggingService _logger = logger;
    }
}
