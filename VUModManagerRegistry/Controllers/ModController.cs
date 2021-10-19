using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VUModManagerRegistry.Authentication.Extensions;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;
using VUModManagerRegistry.Models.Extensions;
using VUModManagerRegistry.Services;
using VUModManagerRegistry.Services.Contracts;

namespace VUModManagerRegistry.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("/api/v{version:apiVersion}/mods/{name}")]
    public class ModController : ControllerBase
    {
        private readonly IModService _modService;
        private readonly IModUploadService _uploadService;
        private readonly IModAuthorizationService _modAuthorizationService;
        private readonly IAuthorizationService _authorizationService;

        public ModController(
            IModService modService,
            IModUploadService uploadService,
            IAuthorizationService authorizationService,
            IModAuthorizationService modAuthorizationService)
        {
            _modService = modService;
            _uploadService = uploadService;
            _authorizationService = authorizationService;
            _modAuthorizationService = modAuthorizationService;
        }

        [HttpGet]
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

        [HttpGet("{modVersion}")]
        public async Task<ActionResult<ModVersionDto>> GetModVersion(string name, string modVersion)
        {
            var foundVersion = await _modService.GetModVersion(name, modVersion);
            if (foundVersion == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, foundVersion, ModOperations.Read);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return foundVersion.ToDto();
        }

        [HttpGet("{modVersion}/archive")]
        public async Task<IActionResult> GetModVersionArchive(string name, string modVersion)
        {
            var foundVersion = await _modService.GetModVersion(name, modVersion);
            if (foundVersion == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, foundVersion, ModOperations.Read);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var stream = _uploadService.GetModVersionArchive(foundVersion.Name, foundVersion.Version);
            stream.Position = 0;
            return File(stream, "application/octet-stream", "archive.tar.gz");
        }
        
        
        [HttpPut("{modVersion}")]
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
                    new {name = createdVersion.Name, modVersion = createdVersion.Version},
                    createdVersion.ToDto());
            }
            catch (ModVersionAlreadyExistsException ex)
            {
                ModelState.AddModelError(nameof(modVersionForm.Attributes.Version), ex.Message);
                return Conflict(ModelState);
            }
        }

        [HttpDelete("{modVersion}")]
        [Authorize(Policy = "CanPublish")]
        public async Task<IActionResult> DeleteModVersion(string name, string modVersion)
        {
            var foundVersion = await _modService.GetModVersion(name, modVersion);
            if (foundVersion == null)
            {
                return NotFound();
            }
            
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, foundVersion, ModOperations.Publish);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            await _modService.DeleteModVersion(name, modVersion);

            return NoContent();
        }
        
        [HttpPost("grant")]
        [Authorize(Policy = "CanPublish")]
        public async Task<ActionResult> GrantUserPermission(string name, GrantPermissionDto permissionDto)
        {
            if (permissionDto.Username == User.Identity?.Name)
            {
                ModelState.AddModelError("Username", "Can not change permissions of yourself");
                return ValidationProblem(ModelState);
            }
            
            // Check permissions
            var mod = await _modService.GetMod(name);
            if (mod == null)
            {
                return NotFound();
            }
            
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, mod, ModOperations.Publish);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (!await _modAuthorizationService.SetPermission(mod.Id, permissionDto.Username, permissionDto.Permission))
            {
                ModelState.AddModelError("Username", "User with given username not found");
                return ValidationProblem(ModelState);
            }

            return Ok();
        }
        
        [HttpPost("revoke")]
        [Authorize(Policy = "CanPublish")]
        public async Task<ActionResult> RevokeUserPermission(string name, GrantPermissionDto permissionDto)
        {
            if (permissionDto.Username == User.Identity?.Name)
            {
                ModelState.AddModelError("Username","Can not change permissions of yourself");
                return ValidationProblem(ModelState);
            }
            
            // Check permissions
            var mod = await _modService.GetMod(name);
            if (mod == null)
            {
                return NotFound();
            }
            
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, mod, ModOperations.Publish);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            if (!await _modAuthorizationService.RevokePermissions(mod.Id, permissionDto.Username))
            {
                ModelState.AddModelError("Username", "User with given username not found");
                return ValidationProblem(ModelState);
            }

            return Ok();
        }
    }
}