using System.Collections.Generic;

namespace VUModManagerRegistry.Models.Dtos
{
    public record ModDto
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string Author { get; init; }
        public Dictionary<string, string> Tags { get; init; }
        public Dictionary<string, ModVersionDto> Versions { get; init; }
    }
}