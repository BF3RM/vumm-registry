using System;

namespace VUModManagerRegistry.Models.Dtos
{
    public class UserAccessTokenDto
    {
        public AccessTokenType Type { get; set; }
        public Guid Token { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}