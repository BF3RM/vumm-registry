﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VUModManagerRegistry.Models;

namespace VUModManagerRegistry.Interfaces
{
    public interface IModService
    {
        /// <summary>
        /// Get a mod by it's name
        /// </summary>
        /// <param name="name">mod name</param>
        /// <returns></returns>
        public Task<ModDto> GetMod(string name);

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
        /// <param name="stream">mod version archive stream</param>
        /// <returns></returns>
        public Task<ModVersionDto> CreateModVersion(ModVersionDto modVersion, string tag, Stream stream);

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
        public Task<ModVersionDto> GetModVersion(string name, string version);
        
        /// <summary>
        /// Delete a mod version
        /// </summary>
        /// <param name="name">mod name</param>
        /// <param name="version">mod version</param>
        /// <returns></returns>
        public Task<bool> DeleteModVersion(string name, string version);
    }
}