using System.Collections.Generic;

namespace VUModManagerRegistry.Models.Dtos
{
    public class ModVersionDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
    }
}