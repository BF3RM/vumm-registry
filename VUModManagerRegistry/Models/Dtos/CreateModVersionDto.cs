using System.Collections.Generic;

namespace VUModManagerRegistry.Models.Dtos
{
    public class ModVersionArchive
    {
        public string ContentType { get; set; }
        public string Data { get; set; }
        public int Length { get; set; }
    }
    public class CreateModVersionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Tag { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }

        public ModVersionArchive Archive { get; set; }
        
    }
}