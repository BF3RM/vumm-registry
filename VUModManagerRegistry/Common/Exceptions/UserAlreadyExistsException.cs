using System;
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
        
        protected UserAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}