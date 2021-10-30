using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using IAuthenticationService = VUModManagerRegistry.Common.Interfaces.IAuthenticationService;

namespace VUModManagerRegistry.Authentication
{
    public class AccessTokenHandler : AuthenticationHandler<AccessTokenOptions>
    {
        private readonly IAuthenticationService _authenticationService;

        public AccessTokenHandler(
            IOptionsMonitor<AccessTokenOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, IAuthenticationService authenticationService) : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            
            if (string.IsNullOrEmpty(authorization))
                return AuthenticateResult.NoResult();
            
            if (!Guid.TryParse(authorization, out var accessToken))
                return AuthenticateResult.Fail("Unauthorized");

            var authRes = await _authenticationService.VerifyToken(accessToken);
            if (!authRes.IsValid)
                return AuthenticateResult.Fail("Unauthorized");

            var claims = new Claim[]
            {
                new("Id", authRes.User.Id.ToString()),
                new(ClaimTypes.Name, authRes.User.Username),
                new("TokenType", authRes.TokenType.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
    }
}