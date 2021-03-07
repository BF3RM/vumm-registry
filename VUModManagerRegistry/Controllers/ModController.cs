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
        private RegistryContext _context;
        
        public ModController(RegistryContext context)
        {
            _context = context;
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

            var modVersion = new ModVersion
            {
                Name = modVersionDto.Name,
                Description = modVersionDto.Description,
                Author = modVersionDto.Author,
                Version = modVersionDto.Version,
                Dependencies = modVersionDto.Dependencies
            };

            _context.ModVersions.Add(modVersion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetModVersion),
                new { name = modVersion.Name, version = modVersion.Version },
                VersionToDto(modVersion));
        }

        private static ModVersionDto VersionToDto(ModVersion modVersion) =>
            new ModVersionDto
            {
                Name = modVersion.Name,
                Description = modVersion.Description,
                Author = modVersion.Author,
                Version = modVersion.Version,
                Dependencies = modVersion.Dependencies
            };
    }
}