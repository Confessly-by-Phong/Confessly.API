using Confessly.Contracts.Core;
using Confessly.Domain;
using Confessly.Services;
using Microsoft.AspNetCore.Mvc;

namespace Confessly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ConfesslyBaseController
    {
        private UserServices _userServices;

        public AuthenticationController(UserServices userServices)
        {
            _userServices = userServices;
        }

        /// <summary>
        /// Registers a new user with the provided user information.
        /// </summary>
        /// <remarks>Returns a 200 status code if the user is successfully registered, 400 if the request
        /// is invalid, or 500 if an internal server error occurs.</remarks>
        /// <param name="user">The user information to register. Must contain all required fields for user creation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the registration operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing a <see cref="ConfesslyResponse{User}"/> with the created user if
        /// registration is successful; otherwise, a <see cref="ConfesslyResponse{object}"/> with error details.</returns>
        [HttpPost("register")]
        [ProducesResponseType<ConfesslyResponse<User>>(200)]
        [ProducesResponseType<ConfesslyResponse<object>>(400)]
        [ProducesResponseType<ConfesslyResponse<object>>(500)]
        public async Task<IActionResult> Register([FromBody] User user,
            CancellationToken cancellationToken)
        {
            User createdUser = await _userServices.CreateUser(user, cancellationToken);
            return StatusCode(200, createdUser);
        }
    }
}
