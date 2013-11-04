using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    internal class FibHeapNode<T, K> where K : IComparable
    {
        internal readonly T Element;
        internal ISet<FibHeapNode<T, K>> Children;
        internal FibHeapNode<T, K> Parent;
        internal bool Marked; // marked for pruning if previously lost child
        internal FibHeapHandle<T, K> HandleTo;
        internal K Key;
        internal long Rank; // number of children

        internal FibHeapNode(T Element, K Key)
        {
            this.Element = Element;
            this.Key = Key;
            Children = new HashSet<FibHeapNode<T, K>>();
            Marked = false;
            Rank = 0;
        }
    }
}
