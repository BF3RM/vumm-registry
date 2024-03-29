using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace VUModManagerRegistry.Common.Exceptions
{
    [Serializable]
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string username) :
            base($"A user with username {username} already exists")
        {
        }
        
        [ExcludeFromCodeCoverage]
        protected UserAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}