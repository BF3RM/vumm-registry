using System.Collections.Generic;

namespace VUModManagerRegistry.Models.Dtos
{
    public record ModVersionDto
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string Author { get; init; }
        public string Version { get; init; }
        public Dictionary<string, string> Dependencies { get; init; }
    }
}