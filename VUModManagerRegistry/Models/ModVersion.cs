using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VUModManagerRegistry.Models
{
    [Table("ModVersions")]
    public class ModVersion : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Version { get; set; }
        
        public string Tag { get; set; }

        public Dictionary<string, string> Dependencies { get; set; } = new();
        
        public long ModId { get; set; }
        public Mod Mod { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}