using System.ComponentModel.DataAnnotations;

namespace VUModManagerRegistry.Models.Dtos
{
    public record CredentialsDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; init; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; init; }
        
        public AccessTokenType Type { get; init; }
    }
}