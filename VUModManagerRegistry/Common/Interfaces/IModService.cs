using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;
using VUModManagerRegistry.Models.Dtos;

namespace VUModManagerRegistry.Common.Interfaces
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
        /// Get all mod versions of a given mod
        /// </summary>
        /// <param name="name">mod name</param>
        /// <returns></returns>
        public Task<List<ModVersion>> GetAllModVersions(string name);
        
        /// <summary>
        /// Get all allowed mod versions for a given user
        /// </summary>
        /// <param name="name">mod name</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public Task<List<ModVersion>> GetAllowedModVersions(string name, long userId);

        /// <summary>
        /// Delete a mod by it's name
        /// </summary>
        /// <param name="name">mod name</param>
        /// <returns></returns>
        public Task<bool> DeleteMod(string name);

        /// <summary>
        /// Create a new mod version
        /// </summary>
        /// <param name="request">mod version request</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public Task<ModVersion> CreateModVersion(CreateModVersionRequest request, long userId);

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