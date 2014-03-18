using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
    public class DestinationCollectionComparer : IEqualityComparer<IEnumerable<IDestination>>
    {
        public bool Equals(IEnumerable<IDestination> x, IEnumerable<IDestination> y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(IEnumerable<IDestination> obj)
        {
            if (obj.Count() > 0)
            {
                return obj.First().GetHashCode();
            }
            else
            {
                return obj.GetHashCode();
            }
        }
    }
}
