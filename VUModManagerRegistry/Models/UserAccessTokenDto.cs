using System;

namespace VUModManagerRegistry.Models
{
    public class UserAccessTokenDto
    {
        public AccessTokenType Type { get; }
        public Guid Token { get; }

        public UserAccessTokenDto(UserAccessToken token)
        {
            Type = token.Type;
            Token = token.Token;
        }
    }
}