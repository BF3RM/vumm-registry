using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CredentialsDto credentials)
        {
            try
            {
                var token = await _authenticationService.Register(credentials);
                return Ok(new UserAccessTokenDto(token));
            }
            catch (UserAlreadyExistsException ex)
            {
                ModelState.AddModelError(nameof(CredentialsDto.Username), ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(CredentialsDto credentials)
        {
            var authRes = await _authenticationService.Login(credentials);
            if (!authRes.IsValid)
            {
                return Unauthorized();
            }

            return Ok(new UserAccessTokenDto(authRes.Token));
        }

        // [Authorize]
        // [HttpGet("tokens")]
        // public Task<IActionResult> GetTokens()
        // {
        //     
        // }
        //
        // [Authorize]
        // [HttpDelete("tokens/{id}")]
        // public Task<IActionResult> RevokeToken(long id)
        // {
        //
        // }
    }
}