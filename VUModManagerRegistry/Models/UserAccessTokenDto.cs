using System;

namespace VUModManagerRegistry.Models
{
    public class UserAccessTokenDto
    {
        public AccessTokenType Type { get; set; }
        public Guid Token { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}