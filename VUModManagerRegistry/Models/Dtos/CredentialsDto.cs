using System.ComponentModel.DataAnnotations;

namespace VUModManagerRegistry.Models.Dtos
{
    public class CredentialsDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        public AccessTokenType Type { get; set; }
    }
}