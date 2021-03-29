using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VUModManagerRegistry.Models
{
    public enum AccessTokenType
    {
        ReadOnly,
        Publish
    }
    
    [Table("UserAccessTokens")]
    public class UserAccessToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        public long UserId { get; set; }
        public User User { get; set; }
        
        [Required]
        public Guid Token { get; set; }
        
        [Required]
        public AccessTokenType Type { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }

        public UserAccessToken()
        {
            CreatedAt = DateTime.Now;
        }
    }
}