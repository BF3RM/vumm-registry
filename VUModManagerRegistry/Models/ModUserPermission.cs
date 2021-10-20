using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VUModManagerRegistry.Models
{
    public enum ModPermission
    {
        Read,
        Write
    }
    
    [Table("ModUserPermissions")]
    public class ModUserPermission
    {
        public long ModId { get; set; }
        public Mod Mod { get; set; }
        
        public long UserId { get; set; }
        public User User { get; set; }
        
        [Required]
        public ModPermission Permission { get; set; }
        
        public string Tag { get; set; }
    }
}