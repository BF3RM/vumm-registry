using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Extensions;

namespace VUModManagerRegistry.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ApiControllerBase
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
            var tokens = await _accessTokenService.GetAll(AuthenticatedUser.Id);
            return Ok(tokens.ToDtoList());
        }
        
        [Authorize]
        [HttpDelete("tokens/{accessToken}")]
        public async Task<IActionResult> RevokeToken(Guid accessToken)
        {
            var success = await _accessTokenService.Revoke(AuthenticatedUser.Id, accessToken);
            if (!success)
            {
                return NotFound();
            }
            
            return NoContent();
        }
    }
}