using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Controllers
{
    [Authorize]
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
            // TODO: Solve this with modelbinding...
            name = name.ToLower();
            
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
            // TODO: Solve this with modelbinding...
            name = name.ToLower();
            
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
            // TODO: Solve this with modelbinding...
            name = name.ToLower();
            
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
            // TODO: Solve this with modelbinding...
            name = name.ToLower();
            modVersionForm.Attributes.Name = modVersionForm.Attributes.Name.ToLower();
            
            if (name != modVersionForm.Attributes.Name || version != modVersionForm.Attributes.Version)
            {
                return BadRequest();
            }

            try
            {
                await using var memoryStream = new MemoryStream();
                await modVersionForm.Archive.CopyToAsync(memoryStream);
                var modVersion =
                    await _modService.CreateModVersion(modVersionForm.Attributes, modVersionForm.Tag, memoryStream);

                return CreatedAtAction(
                    nameof(GetModVersion),
                    new {name = modVersion.Name, version = modVersion.Version},
                    modVersion);
            }
            catch (ModVersionAlreadyExistsException ex)
            {
                ModelState.AddModelError(nameof(modVersionForm.Attributes.Version), ex.Message);
                return Conflict();
            }
        }

        [HttpDelete("{name}/{version}")]
        public async Task<IActionResult> DeleteModVersion(string name, string version)
        {
            if (!await _modService.DeleteModVersion(name, version))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}