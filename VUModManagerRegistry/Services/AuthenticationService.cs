using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly RegistryContext _context;
        private readonly IAccessTokenService _accessTokenService;

        public AuthenticationService(RegistryContext context, IAccessTokenService accessTokenService)
        {
            _context = context;
            _accessTokenService = accessTokenService;
        }

        public async Task<UserAccessToken> Register(CredentialsDto credentials)
        {
            if (_context.Users.Any(u => u.Username == credentials.Username))
            {
                throw new UserAlreadyExistsException(credentials.Username);
            }

            var user = new User()
            {
                Username = credentials.Username,
                Password = await EncryptPassword(credentials.Password),
                AccessTokens = new List<UserAccessToken>()
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return await _accessTokenService.Create(user, credentials.Type);
        }

        public async Task<(bool IsValid, UserAccessToken Token)> Login(CredentialsDto credentials)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == credentials.Username);

            if (user == null)
            {
                return (false, null);
            }

            if (!await VerifyPassword(user.Password, credentials.Password))
            {
                return (false, null);
            }

            var token = await _accessTokenService.Create(user, credentials.Type);
            return (true, token);
        }

        private Task<string> EncryptPassword(string password)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.HashPassword(password));
        }

        private Task<bool> VerifyPassword(string hash, string password)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.Verify(password, hash));
        }
    }
}