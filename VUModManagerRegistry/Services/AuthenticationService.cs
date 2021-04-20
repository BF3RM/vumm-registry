using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;
using VUModManagerRegistry.Repositories;
using VUModManagerRegistry.Repositories.Contracts;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccessTokenRepository _accessTokenRepository;

        public AuthenticationService(IUserRepository userRepository, IAccessTokenRepository accessTokenRepository)
        {
            _userRepository = userRepository;
            _accessTokenRepository = accessTokenRepository;
        }

        public async Task<UserAccessToken> Register(CredentialsDto credentials)
        {
            if (await _userRepository.ExistsByUsernameAsync(credentials.Username))
            {
                throw new UserAlreadyExistsException(credentials.Username);
            }

            var user = new User()
            {
                Username = credentials.Username,
                Password = await EncryptPassword(credentials.Password)
            };

            await _userRepository.AddAsync(user);

            return await CreateAccessToken(user.Id, credentials.Type);
        }

        public async Task<(bool IsValid, UserAccessToken Token)> Login(CredentialsDto credentials)
        {
            var user = await _userRepository.FindByUsernameAsync(credentials.Username);

            if (user == null)
            {
                return (false, null);
            }

            if (!await VerifyPassword(user.Password, credentials.Password))
            {
                return (false, null);
            }

            var token = await CreateAccessToken(user.Id, credentials.Type);
            return (true, token);
        }

        public async Task<(bool IsValid, User User, AccessTokenType TokenType)> VerifyToken(Guid token)
        {
            var accessToken = await _accessTokenRepository.FindByTokenAsync(token);
            if (accessToken == null)
            {
                return (false, null, AccessTokenType.Readonly);
            }

            var user = await _userRepository.FindByIdAsync(accessToken.UserId);

            return (true, user, accessToken.Type);
        }

        private async Task<UserAccessToken> CreateAccessToken(long userId, AccessTokenType type)
        {
            var accessToken = new UserAccessToken()
            {
                Type = type,
                Token = Guid.NewGuid(),
                UserId = userId
            };

            return await _accessTokenRepository.AddAsync(accessToken);
        }
        
        private static Task<string> EncryptPassword(string password)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.HashPassword(password));
        }

        private static Task<bool> VerifyPassword(string hash, string password)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.Verify(password, hash));
        }
    }
}