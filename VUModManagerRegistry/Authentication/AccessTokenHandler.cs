using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using VUModManagerRegistry.Interfaces;

namespace VUModManagerRegistry.Authentication
{
    public class AccessTokenHandler : AuthenticationHandler<AccessTokenOptions>
    {
        private readonly IAccessTokenService _accessTokenService;
        
        public AccessTokenHandler(
            IOptionsMonitor<AccessTokenOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAccessTokenService accessTokenService) : base(options, logger, encoder, clock)
        {
            _accessTokenService = accessTokenService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            
            if (string.IsNullOrEmpty(authorization))
                return AuthenticateResult.NoResult();
            
            if (!Guid.TryParse(authorization, out var accessToken))
                return AuthenticateResult.Fail("Unauthorized");

            var authRes = await _accessTokenService.Verify(accessToken);
            if (!authRes.IsValid)
                return AuthenticateResult.Fail("Unauthorized");

            // var claims = new List<Claim>
            // {
            //     new(ClaimTypes.Sid, accessToken.ToString()),
            //     new(ClaimTypes.Name, authRes.User.Username)
            // };
            // var identity = new ClaimsIdentity(claims, Scheme.Name);
            var identity = new UserIdentity(authRes.User);
            var principal = new GenericPrincipal(identity, null);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
    }
}