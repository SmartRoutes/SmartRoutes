using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    internal class FibHeapNode<T>
    {
        internal readonly T Element;
        internal ISet<FibHeapNode<T>> Children;
        internal FibHeapNode<T> Parent;
        internal bool Marked; // marked for pruning if previously lost child
        internal FibHeapHandle<T> HandleTo;
        internal double Key;
        internal double Rank;

        internal FibHeapNode(T Element, double Key)
        {
            this.Element = Element;
            this.Key = Key;
            Children = new HashSet<FibHeapNode<T>>();
            Marked = false;
        }
    }
}
