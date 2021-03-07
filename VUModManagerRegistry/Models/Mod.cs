using System.Collections.Generic;

namespace VUModManagerRegistry.Models
{
    public class Mod
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        // public Dictionary<string, string> Tags { get; set; }
    }
}