using Confessly.Contracts.Authentication;
using Confessly.Contracts.Core;
using Confessly.Domain;
using Confessly.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace Confessly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ConfesslyBaseController
    {
        private IAuthenticationServices _authenticationServices;

        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
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
        public async Task<IActionResult> Register([FromBody] UserCreate user,
            CancellationToken cancellationToken)
        {
            User createdUser = await _authenticationServices.CreateUser(user, cancellationToken);
            return ApiOk(createdUser);
        }

        /// <summary>
        /// Authenticates a user with the provided login credentials and returns a JWT token.
        /// </summary>
        /// <remarks>Returns a 200 status code with a JWT token if authentication is successful, 400 if the request
        /// is invalid or credentials are incorrect, or 500 if an internal server error occurs.</remarks>
        /// <param name="userLogin">The user login credentials containing username and password for authentication.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the authentication operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing a JWT token wrapped in an anonymous object if
        /// authentication is successful; otherwise, a <see cref="ConfesslyResponse{object}"/> with error details.</returns>
        [HttpPost("login")]
        [ProducesResponseType<string>(200)]
        [ProducesResponseType<ConfesslyResponse<object>>(400)]
        [ProducesResponseType<ConfesslyResponse<object>>(500)]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin,
            CancellationToken cancellationToken)
        {
            string token = await _authenticationServices.AuthenticateUser(userLogin.Username, userLogin.Password, cancellationToken);
            return ApiOk(new { Token = token });
        }
    }
}
