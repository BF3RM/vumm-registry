using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VUModManagerRegistry.Authentication.Extensions;
using VUModManagerRegistry.Common.Exceptions;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;
using VUModManagerRegistry.Models.Extensions;
using VUModManagerRegistry.Services;

namespace VUModManagerRegistry.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("/api/v{version:apiVersion}/mods/{name}")]
    public class ModController : ControllerBase
    {
        private readonly IModService _modService;
        private readonly IModStorage _modStorage;
        private readonly IModAuthorizationService _modAuthorizationService;
        private readonly IAuthorizationService _authorizationService;

        public ModController(
            IModService modService,
            IModStorage modStorage,
            IAuthorizationService authorizationService,
            IModAuthorizationService modAuthorizationService)
        {
            _modService = modService;
            _modStorage = modStorage;
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
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            
            // Filter out non supported versions
            mod.Versions = await _modService.GetAllowedModVersions(mod.Name, User.Id());

            return mod.ToDto();
        }

        [HttpGet("{modVersion}")]
        public async Task<ActionResult<ModVersionDto>> GetModVersion(string name, string modVersion)
        {
            var foundVersion = await _modService.GetModVersion(name, modVersion);
            if (foundVersion == null)
            {
                return NotFound();
            }

            var authorizationResult =
                await _authorizationService.AuthorizeAsync(User, foundVersion, ModOperations.Read);
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return foundVersion.ToDto();
        }

        [HttpPut("{modVersion}")]
        [Authorize(Policy = "CanPublish")]
        public async Task<ActionResult<ModVersionDto>> PutModVersion(string name, string modVersion, CreateModVersionRequest request)
        {
            name = name.ToLower();
            request.Name = request.Name.ToLower();
            
            if (name != request.Name || modVersion != request.Version)
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
                var createdVersion =
                    await _modService.CreateModVersion(request, User.Id());

                return CreatedAtAction(
                    nameof(GetModVersion),
                    new {name = createdVersion.Name, modVersion = createdVersion.Version},
                    createdVersion.ToDto());
            }
            catch (ModVersionAlreadyExistsException ex)
            {
                ModelState.AddModelError(nameof(request.Version), ex.Message);
                return Conflict(ModelState);
            }
        }
        
        [HttpGet("{modVersion}/download")]
        public async Task<ActionResult<ModVersionUrlDto>> GetModVersionDownloadUrl(string name, string modVersion)
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
            
            return new ModVersionUrlDto
            {
                Url = _modStorage.GetArchiveDownloadLink(foundVersion.Name, foundVersion.Version)
            };
        }

        [HttpDelete("{modVersion}")]
        [Authorize(Policy = "CanPublish")]
        public async Task<IActionResult> DeleteModVersion(string name, string modVersion)
        {
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
            
            if (!await _modService.DeleteModVersion(name, modVersion))
            {
                return NotFound();
            }

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

            if (!await _modAuthorizationService.SetPermission(mod.Id, permissionDto.Username, permissionDto.Tag, permissionDto.Permission))
            {
                ModelState.AddModelError("Username", "User with given username not found");
                return ValidationProblem(ModelState);
            }

            return Ok();
        }
        
        [HttpPost("revoke")]
        [Authorize(Policy = "CanPublish")]
        public async Task<ActionResult> RevokeUserPermission(string name, RevokePermissionDto permissionDto)
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

            if (!await _modAuthorizationService.RevokePermissions(mod.Id, permissionDto.Username, permissionDto.Tag))
            {
                ModelState.AddModelError("Username", "User with given username not found");
                return ValidationProblem(ModelState);
            }

            return Ok();
        }
    }
}