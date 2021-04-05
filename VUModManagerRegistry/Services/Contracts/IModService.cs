using System.IO;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;

namespace VUModManagerRegistry.Services.Contracts
{
    public interface IModService
    {
        /// <summary>
        /// Get a mod by it's name
        /// </summary>
        /// <param name="name">mod name</param>
        /// <returns></returns>
        public Task<Mod> GetMod(string name);

        /// <summary>
        /// Delete a mod by it's name
        /// </summary>
        /// <param name="name">mod name</param>
        /// <returns></returns>
        public Task<bool> DeleteMod(string name);

        /// <summary>
        /// Create a new mod version
        /// </summary>
        /// <param name="modVersion">mod version</param>
        /// <param name="tag">mod version tag</param>
        /// <param name="userId">user id</param>
        /// <param name="stream">mod version archive stream</param>
        /// <returns></returns>
        public Task<ModVersion> CreateModVersion(ModVersionDto modVersion, string tag, long userId, Stream stream);

        /// <summary>
        /// Check whether a mod version exists
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public Task<bool> ModVersionExists(string name, string version);
        
        /// <summary>
        /// Get mod version
        /// </summary>
        /// <param name="name">mod name</param>
        /// <param name="version">mod version</param>
        /// <returns></returns>
        public Task<ModVersion> GetModVersion(string name, string version);
        
        /// <summary>
        /// Delete a mod version
        /// </summary>
        /// <param name="name">mod name</param>
        /// <param name="version">mod version</param>
        /// <returns></returns>
        public Task<bool> DeleteModVersion(string name, string version);
    }
}