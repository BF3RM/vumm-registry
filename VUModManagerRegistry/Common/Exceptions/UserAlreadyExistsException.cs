using System;

namespace VUModManagerRegistry.Common.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string username) :
            base($"A user with username {username} already exists")
        {
        }
    }
}