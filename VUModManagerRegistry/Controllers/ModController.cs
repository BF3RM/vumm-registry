using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Controllers
{
    [ApiController]
    [Route("/api/mods")]
    public class ModController : ControllerBase
    {
        private readonly IModService _modService;
        private readonly IModUploadService _uploadService;

        public ModController(IModService modService, IModUploadService uploadService)
        {
            _modService = modService;
            _uploadService = uploadService;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<ModDto>> GetMod(string name)
        {
            var mod = await _modService.GetMod(name);
            if (mod == null)
            {
                return NotFound();
            }

            return mod;
        }
        
        [HttpGet("{name}/{version}")]
        public async Task<ActionResult<ModVersionDto>> GetModVersion(string name, string version)
        {
            var modVersion = await _modService.GetModVersion(name, version);
            if (modVersion == null)
            {
                return NotFound();
            }

            return modVersion;
        }

        [HttpGet("{name}/{version}/archive")]
        public async Task<IActionResult> GetModVersionArchive(string name, string version)
        {
            if (!await _modService.ModVersionExists(name, version))
            {
                return NotFound();
            }

            var stream = _uploadService.GetModVersionArchive(name, version);
            stream.Position = 0;
            return File(stream, "application/octet-stream", "archive.tar.gz");
        }
        
        
        [HttpPut("{name}/{version}")]
        public async Task<ActionResult<ModVersionDto>> PutModVersion(string name, string version, [FromForm]ModVersionForm modVersionForm)
        {
            if (name != modVersionForm.Attributes.Name || version != modVersionForm.Attributes.Version)
            {
                return BadRequest();
            }
            
            await using var memoryStream = new MemoryStream();
            await modVersionForm.Archive.CopyToAsync(memoryStream);
            var modVersion = await _modService.CreateModVersion(name, version, modVersionForm.Attributes.Dependencies, memoryStream);

            return CreatedAtAction(
                nameof(GetModVersion),
                new { name = modVersion.Name, version = modVersion.Version },
                modVersion);
        }
    }
}