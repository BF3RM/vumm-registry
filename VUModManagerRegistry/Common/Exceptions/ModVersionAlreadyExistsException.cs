using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VUModManagerRegistry.Common.Exceptions
{
    [Serializable]
    public class ModVersionAlreadyExistsException : Exception
    {
        public ModVersionAlreadyExistsException(string mod, string version)
            : base($"Mod {mod} already has version {version} registered")
        {
        }

        [ExcludeFromCodeCoverage]
        protected ModVersionAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}