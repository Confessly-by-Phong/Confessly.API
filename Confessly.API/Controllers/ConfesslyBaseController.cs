using Confessly.Contracts.Core;
using Microsoft.AspNetCore.Mvc;

namespace Confessly.API.Controllers
{
    [ApiController]
    public class ConfesslyBaseController : ControllerBase
    {
        protected IActionResult ApiOk<T>(T data)
        {
            return Ok(ConfesslyResponse<T>.Ok(data, HttpContext.TraceIdentifier));
        }
    }
}
