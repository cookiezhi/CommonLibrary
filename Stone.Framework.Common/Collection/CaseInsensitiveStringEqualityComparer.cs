using System;
using System.Collections.Generic;

namespace Stone.Framework.Common.Collection
{
    public class CaseInsensitiveStringEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.ToUpper().GetHashCode();
        }
    }
}