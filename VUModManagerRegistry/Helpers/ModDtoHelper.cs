using System.Linq;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Helpers
{
    public static class ModDtoHelper
    {
        public static ModDto ModToDto(Mod mod) =>
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
                    .ToDictionary(v => v.Version, ModVersionToDto)
            };
        
        public static ModVersionDto ModVersionToDto(ModVersion modVersion) =>
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