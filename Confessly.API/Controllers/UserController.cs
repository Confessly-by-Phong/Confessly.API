using Confessly.Services;
using Microsoft.AspNetCore.Mvc;

namespace Confessly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ConfesslyBaseController
    {
        private UserServices _userServices;

        public UserController(UserServices userServices)
        {
            _userServices = userServices;
        }
    }
}
