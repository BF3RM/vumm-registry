using System;

namespace VUModManagerRegistry.Exceptions
{
    public class ModVersionAlreadyExistsException : Exception
    {
        public ModVersionAlreadyExistsException(string mod, string version)
            : base($"Mod {mod} already has version {version} registered")
        {
        }
    }
}