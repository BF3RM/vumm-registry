using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace VUModManagerRegistry.Models
{
    [Table("Users")]
    public class User : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public List<UserAccessToken> AccessTokens { get; set; }
        
        public virtual ICollection<Mod> Mods { get; set; }
        public virtual ICollection<ModUserPermission> ModPermissions { get; set; }
    }
}