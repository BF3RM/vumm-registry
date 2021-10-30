﻿using System.Collections.Generic;
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
        private readonly IModAuthorizationService _modAuthorizationService;
        private readonly IModUploadService _uploadService;

        public ModService(IModRepository modRepository, IModVersionRepository modVersionRepository, IModAuthorizationService modAuthorizationService, IModUploadService uploadService)
        {
            _modRepository = modRepository;
            _modVersionRepository = modVersionRepository;
            _modAuthorizationService = modAuthorizationService;
            _uploadService = uploadService;
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

        public async Task<ModVersion> CreateModVersion(ModVersionDto modVersionDto, string tag, long userId,
            Stream stream)
        {
            if (await ModVersionExists(modVersionDto.Name, modVersionDto.Version))
            {
                throw new ModVersionAlreadyExistsException(modVersionDto.Name, modVersionDto.Version);
            }

            // Get or create the mod
            var mod = await _CreateOrGetMod(modVersionDto.Name, userId);
            
            // Create mod version
            var modVersion = new ModVersion
            {
                Name = modVersionDto.Name,
                Version = modVersionDto.Version,
                Dependencies = modVersionDto.Dependencies,
                Tag = tag,
                ModId = mod.Id
            };
            await _modVersionRepository.AddAsync(modVersion);

            // Upload the archive
            await _uploadService.StoreModVersionArchive(modVersionDto.Name, modVersionDto.Version, stream);

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

            _uploadService.DeleteModVersionArchive(name, version);

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