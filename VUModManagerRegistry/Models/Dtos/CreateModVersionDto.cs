using System.Collections.Generic;

namespace VUModManagerRegistry.Models.Dtos
{
    public record ModVersionArchive
    {
        public string ContentType { get; init; }
        public string Data { get; init; }
        public int Length { get; init; }
    }
    
    public record CreateModVersionRequest
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string Author { get; init; }
        public string Version { get; init; }
        public string Tag { get; init; }
        public Dictionary<string, string> Dependencies { get; init; }
        public ModVersionArchive Archive { get; init; }
    }
}