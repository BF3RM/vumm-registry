using System;

namespace VUModManagerRegistry.Models.Dtos
{
    public record UserAccessTokenDto
    {
        public AccessTokenType Type { get; init; }
        public Guid Token { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}