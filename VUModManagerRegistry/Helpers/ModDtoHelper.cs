using System.Linq;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Helpers
{
    public class ModDtoHelper
    {
        public static ModDto ModToDto(Mod mod) =>
            new()
            {
                Name = mod.Name,
                Description = mod.Description,
                Author = mod.Author,
                Tags = mod.Tags.ToDictionary(t => t.Name, t => t.Version),
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