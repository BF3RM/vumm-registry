using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VUModManagerRegistry.Authentication.Extensions;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Extensions;
using VUModManagerRegistry.Services;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("/api/v{version:apiVersion}/mods")]
    public class ModController : ControllerBase
    {
        private readonly IModService _modService;
        private readonly IModUploadService _uploadService;
        private readonly IAuthorizationService _authorizationService;

        public ModController(
            IModService modService,
            IModUploadService uploadService,
            IAuthorizationService authorizationService)
        {
            _modService = modService;
            _uploadService = uploadService;
            _authorizationService = authorizationService;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<ModDto>> GetMod(string name)
        {
            var mod = await _modService.GetMod(name.ToLower());
            if (mod == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, mod, ModOperations.Read);
            if (authorizationResult.Succeeded)
            {
                return mod.ToDto();
            }

            return Forbid();
        }

        [HttpGet("{name}/{modVersion}")]
        public async Task<ActionResult<ModVersionDto>> GetModVersion(string name, string modVersion)
        {
            var mod = await _modService.GetMod(name.ToLower());
            if (mod == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, mod, ModOperations.Read);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var foundVersion = await _modService.GetModVersion(mod.Name, modVersion);
            if (foundVersion == null)
            {
                return NotFound();
            }

            return foundVersion.ToDto();
        }

        [HttpGet("{name}/{modVersion}/archive")]
        public async Task<IActionResult> GetModVersionArchive(string name, string modVersion)
        {
            var mod = await _modService.GetMod(name.ToLower());
            if (mod == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, mod, ModOperations.Read);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            
            if (!await _modService.ModVersionExists(mod.Name, modVersion))
            {
                return NotFound();
            }

            var stream = _uploadService.GetModVersionArchive(mod.Name, modVersion);
            stream.Position = 0;
            return File(stream, "application/octet-stream", "archive.tar.gz");
        }
        
        
        [HttpPut("{name}/{modVersion}")]
        [Authorize(Policy = "CanPublish")]
        public async Task<ActionResult<ModVersionDto>> PutModVersion(string name, string modVersion, [FromForm]ModVersionForm modVersionForm)
        {
            name = name.ToLower();
            modVersionForm.Attributes.Name = modVersionForm.Attributes.Name.ToLower();
            
            if (name != modVersionForm.Attributes.Name || modVersion != modVersionForm.Attributes.Version)
            {
                return BadRequest();
            }

            // Check permissions
            var mod = await _modService.GetMod(name);
            if (mod != null)
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, mod, ModOperations.Publish);
                if (!authorizationResult.Succeeded)
                {
                    return Forbid();
                }
            }

            try
            {
                await using var memoryStream = new MemoryStream();
                await modVersionForm.Archive.CopyToAsync(memoryStream);
                var createdVersion =
                    await _modService.CreateModVersion(modVersionForm.Attributes, modVersionForm.Tag, User.Id(), memoryStream);

                return CreatedAtAction(
                    nameof(GetModVersion),
                    new {name = createdVersion.Name, version = createdVersion.Version},
                    createdVersion.ToDto());
            }
            catch (ModVersionAlreadyExistsException ex)
            {
                ModelState.AddModelError(nameof(modVersionForm.Attributes.Version), ex.Message);
                return Conflict(ModelState);
            }
        }

        [HttpDelete("{name}/{modVersion}")]
        public async Task<IActionResult> DeleteModVersion(string name, string modVersion)
        {
            // Check permissions
            var mod = await _modService.GetMod(name);
            if (mod != null)
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, mod, ModOperations.Publish);
                if (!authorizationResult.Succeeded)
                {
                    return Forbid();
                }
            }
            
            if (!await _modService.DeleteModVersion(name, modVersion))
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}