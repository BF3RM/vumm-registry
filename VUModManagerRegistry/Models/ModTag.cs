using System.ComponentModel.DataAnnotations;

namespace VUModManagerRegistry.Models
{
    public class ModTag
    {
        [Key]
        public long Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Version { get; set; }
        
        public long ModId { get; set; }
    }
}