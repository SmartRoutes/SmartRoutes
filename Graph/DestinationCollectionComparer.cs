using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model;

namespace SmartRoutes.Graph
{
    // equality comparer for type IEnumerable<IDestination>
    // 
    public class DestinationCollectionComparer : IEqualityComparer<IEnumerable<IDestination>>
    {
        public bool Equals(IEnumerable<IDestination> x, IEnumerable<IDestination> y)
        {
            bool areEqual = true;

            if (x.Count() != y.Count())
            {
                areEqual = false;
            }
            else if (areEqual)
            {
                foreach (var dest in x)
                {
                    if (!y.Contains(dest))
                    {
                        areEqual = false;
                        break;
                    }
                }
            }
            else if (areEqual)
            {
                foreach (var dest in y)
                {
                    if (!x.Contains(dest))
                    {
                        areEqual = false;
                        break;
                    }
                }
            }

            return areEqual;
        }

        public int GetHashCode(IEnumerable<IDestination> obj)
        {
            if (obj.Count() > 0)
            {
                int hash = 17;

                foreach (var dest in obj)
                {
                    hash = hash * 23 + dest.GetHashCode();
                }

                return hash;
            }
            else
            {
                return obj.GetHashCode();
            }
        }
    }
}
