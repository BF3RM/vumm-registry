using System.Linq;

namespace VUModManagerRegistry.Models.Extensions
{
    public static class ModExtensions
    {
        public static ModDto ToDto(this Mod mod) =>
            new()
            {
                Name = mod.Name,
                Description = mod.Description,
                Author = mod.Author,
                Tags = mod.Versions
                    .GroupBy(v => v.Tag)
                    .Select(g => g.Last())
                    .ToDictionary(v => v.Tag, v => v.Version),
                Versions = mod.Versions
                    .ToDictionary(v => v.Version, v => v.ToDto())
            };
        
        public static ModVersionDto ToDto(this ModVersion modVersion) =>
            new()
            {
                Name = modVersion.Name,
                Description = modVersion.Description,
                Author = modVersion.Author,
                Version = modVersion.Version,
                Dependencies = modVersion.Dependencies
            };
    }
}