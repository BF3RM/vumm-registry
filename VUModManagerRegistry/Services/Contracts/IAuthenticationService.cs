using System;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services.Contracts
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Register a new user and return newly created access token
        /// </summary>
        /// <param name="credentials">credentials to register with</param>
        /// <returns></returns>
        public Task<UserAccessToken> Register(CredentialsDto credentials);

        /// <summary>
        /// Login using given credentials and give back an access token
        /// </summary>
        /// <param name="credentials">credentials to login with</param>
        /// <returns></returns>
        public Task<(bool IsValid, UserAccessToken Token)> Login(CredentialsDto credentials);

        /// <summary>
        /// Login using an access token and give back correlating user
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<(bool IsValid, User User)> VerifyToken(Guid token);
    }
}