﻿using System.Collections.Generic;
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
        
        public List<ModVersion> Versions { get; set; }
        
        public ICollection<User> Users { get; set; }
        public List<ModUserPermission> UserPermissions { get; set; }
    }
}