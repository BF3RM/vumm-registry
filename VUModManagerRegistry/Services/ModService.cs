using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VUModManagerRegistry.Exceptions;
using VUModManagerRegistry.Helpers;
using VUModManagerRegistry.Interfaces;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Services
{
    public class ModService : IModService
    {
        private readonly AppDbContext _context;
        private readonly IModUploadService _uploadService;

        public ModService(AppDbContext context, IModUploadService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        public async Task<Mod> GetMod(string name)
        {
            var mod = await _context.Mods
                .Include(m => m.Versions)
                .AsSingleQuery()
                .SingleOrDefaultAsync(m => m.Name == name);

            return mod;
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

        public async Task<ModVersion> CreateModVersion(ModVersionDto modVersionDto, string tag, Stream stream)
        {
            if (await ModVersionExists(modVersionDto.Name, modVersionDto.Version))
            {
                throw new ModVersionAlreadyExistsException(modVersionDto.Name, modVersionDto.Version);
            }

            // Get or create the mod
            var mod = await _CreateOrGetMod(modVersionDto.Name);
            
            // Create mod version
            var modVersion = new ModVersion
            {
                Name = modVersionDto.Name,
                Version = modVersionDto.Version,
                Dependencies = modVersionDto.Dependencies,
                Tag = tag,
                ModId = mod.Id
            };
            await _context.ModVersions.AddAsync(modVersion);

            // Save changes
            await _context.SaveChangesAsync();
            
            // Upload the archive
            await _uploadService.StoreModVersionArchive(modVersionDto.Name, modVersionDto.Version, stream);

            return modVersion;
        }

        public Task<bool> ModVersionExists(string name, string version)
        {
            return _context.ModVersions.AnyAsync(m => m.Name == name && m.Version == version);
        }

        public async Task<ModVersion> GetModVersion(string name, string version)
        {
            var modVersion = await _context.ModVersions
                .SingleOrDefaultAsync(m => m.Name == name && m.Version == version);

            return modVersion;
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
            
            // Delete mod if no versions are left
            var mod = await _context.Mods.Include(m => m.Versions).SingleOrDefaultAsync(m => m.Name == name);
            if (mod.Versions.Count == 0)
            {
                _context.Mods.Remove(mod);
                await _context.SaveChangesAsync();
            }

            await _uploadService.DeleteModVersionArchive(name, version);

            return true;
        }

        private async Task<Mod> _CreateOrGetMod(string name)
        {
            var mod = await _context.Mods.SingleOrDefaultAsync(m => m.Name == name);
            if (mod == null)
            {
                // If mod does not exist yet, create a new one
                mod = new Mod
                {
                    Name = name
                };
                await _context.Mods.AddAsync(mod);
                await _context.SaveChangesAsync();
            }

            return mod;
        }
    }
}