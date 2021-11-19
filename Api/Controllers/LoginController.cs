using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "User")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserDetailsProvider _userDetailsProvider;
        private readonly IAuthenticatedSessionProvider _authenticationProvider;

        public LoginController(ILogger<WeatherForecastController> logger,
            UserDetailsProvider userDetailsProvider,
            IAuthenticatedSessionProvider authenticationProvider)
        {
            _logger = logger;
            _userDetailsProvider = userDetailsProvider;
            _authenticationProvider = authenticationProvider;
        }

        [HttpPost("external-login")]
        public void ExternalLogin()
        {
            // Do nothing.
        }

        [HttpPost("who-am-i")]
        [AllowAnonymous]
        public LoginReply WhoAmI()
        {
            if (_userDetailsProvider.UserDetails == null)
            {
                return null;
            }

            return new LoginReply
            {
                UserId = _userDetailsProvider.UserDetails.UserId,
            };
        }

        [HttpPost("logout")]
        public async Task Logout()
        {
            await _authenticationProvider.EndSessionAsync();
        }
    }
}