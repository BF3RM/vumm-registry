using System;

namespace VUModManagerRegistry.Common.Interfaces
{
    public interface ISystemTimeProvider
    {
        DateTime Now { get; }
    }
}