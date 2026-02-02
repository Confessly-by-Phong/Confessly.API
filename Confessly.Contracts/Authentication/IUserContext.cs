using System;
using System.Collections.Generic;
using System.Text;

namespace Confessly.Contracts.Authentication
{
    public interface IUserContext
    {
        Guid GetCurrentUserId();
    }
}
