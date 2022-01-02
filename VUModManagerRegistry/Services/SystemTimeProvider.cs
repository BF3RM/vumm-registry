using System;
using VUModManagerRegistry.Common.Interfaces;

namespace VUModManagerRegistry.Services
{
    public class SystemTimeProvider : ISystemTimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}