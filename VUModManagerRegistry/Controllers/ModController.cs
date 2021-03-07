using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Controllers
{
    [ApiController]
    [Route("/api/mods")]
    public class ModController : ControllerBase
    {
        private readonly RegistryContext _context;
        
        public ModController(RegistryContext context)
        {
            _context = context;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<ModDto>> GetMod(string name)
        {
            var mod = await _context.Mods
                .Include(m => m.Versions)
                .Include(m => m.Tags)
                .SingleOrDefaultAsync(m => m.Name == name);
            
            if (mod == null)
            {
                return NotFound();
            }

            return ModToDto(mod);
        }
        
        [HttpGet("{name}/{version}")]
        public async Task<ActionResult<ModVersionDto>> GetModVersion(string name, string version)
        {
            var modVersion = await _context.ModVersions
                .FirstOrDefaultAsync(v => v.Name == name && v.Version == version);

            if (modVersion == null)
            {
                return NotFound();
            }

            return VersionToDto(modVersion);
        }
        
        [HttpPut("{name}/{version}")]
        public async Task<ActionResult<ModVersionDto>> PutModVersion(string name, string version, ModVersionDto modVersionDto)
        {
            if (name != modVersionDto.Name || version != modVersionDto.Version)
            {
                return BadRequest();
            }

            var mod = await _context.Mods.SingleOrDefaultAsync(m => m.Name == name);
            if (mod == null)
            {
                mod = new Mod
                {
                    Name = name,
                    Description = modVersionDto.Description,
                    Author = modVersionDto.Author
                };
                _context.Mods.Add(mod);
            }

            var modVersion = new ModVersion
            {
                Name = modVersionDto.Name,
                Description = modVersionDto.Description,
                Author = modVersionDto.Author,
                Version = modVersionDto.Version,
                Dependencies = modVersionDto.Dependencies,
                ModId = mod.Id
            };

            _context.ModVersions.Add(modVersion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetModVersion),
                new { name = modVersion.Name, version = modVersion.Version },
                VersionToDto(modVersion));
        }

        private static ModDto ModToDto(Mod mod) =>
            new()
            {
                Name = mod.Name,
                Description = mod.Description,
                Author = mod.Author,
                Tags = mod.Tags.ToDictionary(t => t.Name, t => t.Version),
                Versions = mod.Versions
                    .ToDictionary(v => v.Version, VersionToDto)
            };
        
        private static ModVersionDto VersionToDto(ModVersion modVersion) =>
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