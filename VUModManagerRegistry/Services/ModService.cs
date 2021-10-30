using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VUModManagerRegistry.Common.Exceptions;
using VUModManagerRegistry.Common.Interfaces;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;

namespace VUModManagerRegistry.Services
{
    public class ModService : IModService
    {
        private readonly IModRepository _modRepository;
        private readonly IModVersionRepository _modVersionRepository;
        private readonly IModStorage _modStorage;
        private readonly IModAuthorizationService _modAuthorizationService;

        public ModService(
            IModRepository modRepository,
            IModVersionRepository modVersionRepository,
            IModStorage modStorage,
            IModAuthorizationService modAuthorizationService)
        {
            _modRepository = modRepository;
            _modVersionRepository = modVersionRepository;
            _modStorage = modStorage;
            _modAuthorizationService = modAuthorizationService;
        }

        public Task<Mod> GetMod(string name)
        {
            return _modRepository.FindByNameAsync(name);
        }

        public Task<List<ModVersion>> GetAllowedModVersions(string name, long userId)
        {
            return _modVersionRepository.FindAllowedVersions(name, userId);
        }

        public Task<bool> DeleteMod(string name)
        {
            return _modRepository.DeleteByNameAsync(name);
        }

        public async Task<ModVersion> CreateModVersion(CreateModVersionRequest request, long userId)
        {
            if (await ModVersionExists(request.Name, request.Version))
            {
                throw new ModVersionAlreadyExistsException(request.Name, request.Version);
            }

            // Get or create the mod
            var mod = await _CreateOrGetMod(request.Name, userId);

            // TODO: Check if length is the same as reported by cli.
            var bytes = Convert.FromBase64String(request.Archive.Data);
            await using (var stream = new MemoryStream(bytes))
            {
                await _modStorage.StoreArchive(mod.Name, request.Version, stream);
            }

            // Create mod version
            var modVersion = new ModVersion
            {
                Name = request.Name,
                Version = request.Version,
                Dependencies = request.Dependencies,
                Tag = request.Tag,
                ModId = mod.Id
            };
            await _modVersionRepository.AddAsync(modVersion);

            return modVersion;
        }

        public Task<bool> ModVersionExists(string name, string version)
        {
            return _modVersionRepository.ExistsByNameAndVersionAsync(name, version);
        }

        public Task<ModVersion> GetModVersion(string name, string version)
        {
            return _modVersionRepository.FindByNameAndVersion(name, version);
        }

        public async Task<bool> DeleteModVersion(string name, string version)
        {
            await _modStorage.DeleteArchive(name, version);
            
            var deleted = await _modVersionRepository.DeleteByNameAndVersionAsync(name, version);
            if (!deleted)
            {
                return false;
            }

            var mod = await _modRepository.FindByNameWithVersionsAsync(name);
            if (mod.Versions.Count == 0)
            {
                await _modRepository.DeleteByIdAsync(mod.Id);
            }

            return true;
        }

        private async Task<Mod> _CreateOrGetMod(string name, long userId)
        {
            var mod = await _modRepository.FindByNameAsync(name);
            if (mod != null)
                return mod;
            
            // If mod does not exist yet, create a new one
            mod = new Mod
            {
                Name = name
            };
            await _modRepository.AddAsync(mod);
            await _modAuthorizationService.SetPermission(mod.Id, userId, ModPermission.Write);

            return mod;
        }
    }
}