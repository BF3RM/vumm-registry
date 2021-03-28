using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace VUModManagerRegistry.Helpers
{
    public static class DictionaryHelpers
    {
        public static ValueComparer<Dictionary<string, string>> StringValueComparer =>
            new ValueComparer<Dictionary<string, string>>(
                (c1, c2) => c1.Equals(c2),
                c => c.GetHashCode(),
                c => c.ToDictionary(entry => entry.Key, entry => entry.Value));
    }
}