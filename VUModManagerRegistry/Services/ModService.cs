using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Helpers;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class ModService : IModService
    {
        private readonly RegistryContext _context;
        private readonly IModUploadService _uploadService;

        public ModService(RegistryContext context, IModUploadService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        public async Task<ModDto> GetMod(string name)
        {
            var mod = await _context.Mods
                .Include(m => m.Versions)
                .Include(m => m.Tags)
                .AsSingleQuery()
                .SingleOrDefaultAsync(m => m.Name == name);

            return mod == null ? null : ModDtoHelper.ModToDto(mod);
        }

        public async Task<bool> DeleteMod(string name)
        {
            var mod = await _context.Mods.SingleOrDefaultAsync(m => m.Name == name);
            if (mod == null)
            {
                return false;
            }

            _context.Mods.Remove(mod);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ModVersionDto> CreateModVersion(ModVersionDto modVersionDto, string tag, Stream stream)
        {
            if (await ModVersionExists(modVersionDto.Name, modVersionDto.Version))
            {
                throw new ArgumentException("An entry with the same version already exists.");
            }
            
            var mod = await _context.Mods.SingleOrDefaultAsync(m => m.Name == modVersionDto.Name);
            if (mod == null)
            {
                // If mod does not exist yet, create a new one
                mod = new Mod
                {
                    Name = modVersionDto.Name
                };
                _context.Mods.Add(mod);
            }
            
            // Create mod version
            var modVersion = new ModVersion
            {
                Name = modVersionDto.Name,
                Version = modVersionDto.Version,
                Dependencies = modVersionDto.Dependencies,
                ModId = mod.Id
            };
            _context.ModVersions.Add(modVersion);
            
            // Default tag to latest
            if (String.IsNullOrEmpty(tag))
            {
                tag = "latest";
            }

            // Create/update mod tag
            var modTag = await _context.ModTags.SingleOrDefaultAsync(t => t.ModId == mod.Id && t.Name == tag);
            if (modTag == null) 
            {
                modTag = new ModTag()
                {
                    Name = tag,
                    Version = modVersionDto.Version,
                    ModId = mod.Id
                };
                _context.ModTags.Add(modTag);
            }
            else
            {
                modTag.Version = modVersionDto.Version;
            }
            await _context.SaveChangesAsync();
            
            await _uploadService.StoreModVersionArchive(modVersionDto.Name, modVersionDto.Version, stream);
            
            return ModDtoHelper.ModVersionToDto(modVersion);
        }

        public Task<bool> ModVersionExists(string name, string version)
        {
            return _context.ModVersions.AnyAsync(m => m.Name == name && m.Version == version);
        }

        public async Task<ModVersionDto> GetModVersion(string name, string version)
        {
            var modVersion = await _context.ModVersions
                .SingleOrDefaultAsync(m => m.Name == name && m.Version == version);

            return modVersion == null ? null : ModDtoHelper.ModVersionToDto(modVersion);
        }

        public async Task<bool> DeleteModVersion(string name, string version)
        {
            var modVersion = await _context.ModVersions
                .SingleOrDefaultAsync(m => m.Name == name && m.Version == version);
            
            if (modVersion == null)
            {
                return false;
            }

            _context.ModVersions.Remove(modVersion);
            await _context.SaveChangesAsync();

            return true;
        }
        
        
    }
}