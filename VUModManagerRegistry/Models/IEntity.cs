using System;

namespace VUModManagerRegistry.Models
{
    public interface IEntity
    {
        long Id { get; set; }
        
        DateTime Created { get; set; }
        
        DateTime LastUpdated { get; set; }
    }
}