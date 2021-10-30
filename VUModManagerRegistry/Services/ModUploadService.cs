using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using VUModManagerRegistry.Common.Interfaces;

namespace VUModManagerRegistry.Services
{
    [ExcludeFromCodeCoverage]
    public class ModUploadService : IModUploadService
    {
        public async Task StoreModVersionArchive(string name, string version, Stream stream)
        {
            // TODO: escape paths, don't allow ../ for example!
            var modPath = Path.Combine("archive", name);

            var exists = Directory.Exists(modPath);
            if (!exists)
            {
                Directory.CreateDirectory(modPath);
            }

            await using var fileStream = new FileStream(Path.Combine(modPath, $"{version}.tar.gz"), FileMode.Create);
            stream.Position = 0;
            await stream.CopyToAsync(fileStream);
        }

        public Stream GetModVersionArchive(string name, string version)
        {
            // TODO: escape paths, don't allow ../ for example!
            var modArchivePath = Path.Combine("archive", name, $"{version}.tar.gz");

            return File.Open(modArchivePath, FileMode.Open);
        }

        public void DeleteModVersionArchive(string name, string version)
        {
            // TODO: escape paths, don't allow ../ for example!
            var modArchivePath = Path.Combine("archive", name, $"{version}.tar.gz");
            
            File.Delete(modArchivePath);
        }
    }
}