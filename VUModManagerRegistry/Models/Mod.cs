using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VUModManagerRegistry.Models
{
    [Table("Mods")]
    [Index(nameof(Name))]
    public class Mod : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        
        public bool IsPrivate { get; set; }
        
        public virtual ICollection<ModVersion> Versions { get; set; }
        
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ModUserPermission> UserPermissions { get; set; }
    }
}