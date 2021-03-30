using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Authentication.Extensions;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Extensions;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccessTokenService _accessTokenService;

        public AuthController(IAuthenticationService authenticationService, IAccessTokenService accessTokenService)
        {
            _authenticationService = authenticationService;
            _accessTokenService = accessTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CredentialsDto credentials)
        {
            try
            {
                var token = await _authenticationService.Register(credentials);
                return Ok(token.ToDto());
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

            return Ok(authRes.Token.ToDto());
        }

        [Authorize]
        [HttpGet("tokens")]
        public async Task<IActionResult> GetTokens()
        {
            var tokens = await _accessTokenService.GetAll(User.AuthenticatedUser().Id);
            return Ok(tokens.ToDtoList());
        }
        
        [Authorize]
        [HttpDelete("tokens/{accessToken}")]
        public async Task<IActionResult> RevokeToken(Guid accessToken)
        {
            var success = await _accessTokenService.Revoke(User.AuthenticatedUser().Id, accessToken);
            if (!success)
            {
                return NotFound();
            }
            
            return NoContent();
        }
    }
}