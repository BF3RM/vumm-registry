using System.ComponentModel.DataAnnotations;

namespace VUModManagerRegistry.Models
{
    public class CredentialsDto
    {
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(100)]
        [MinLength(6)]
        public string Password { get; set; }
        
        public AccessTokenType Type { get; set; }
    }
}