using System;
using System.Collections.Generic;

namespace Pen.Service
{    
    class HidContactInfoEqualityComparer : IEqualityComparer<HidContactInfo>
    {
        public bool Equals(HidContactInfo x, HidContactInfo y)
        {
            return x.Id.Equals(x.Id);
        }

        public int GetHashCode(HidContactInfo obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
