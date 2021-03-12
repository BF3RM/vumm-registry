﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace VUModManagerRegistry.Interfaces
{
    public interface IModUploadService
    {
        /// <summary>
        /// Store mod version archive
        /// </summary>
        /// <param name="name">mod name</param>
        /// <param name="version">mod version</param>
        /// <param name="stream">archive memory stream</param>
        /// <returns></returns>
        public Task StoreModVersionArchive(string name, string version, Stream stream);

        /// <summary>
        /// Get stream to mod version archive file
        /// </summary>
        /// <param name="name">mod name</param>
        /// <param name="version">mod version</param>
        /// <returns></returns>
        public Stream GetModVersionArchive(string name, string version);
        
        /// <summary>
        /// Delete mod version archive if it exists
        /// </summary>
        /// <param name="name">mod name</param>
        /// <param name="version">mod version</param>
        /// <returns></returns>
        public Task DeleteModVersionArchive(string name, string version);
    }
}