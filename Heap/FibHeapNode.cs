using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public class FibHeapNode<T>
    {
        public readonly T Element;
        public ISet<FibHeapNode<T>> Children;
        public FibHeapNode<T> Parent;
        public bool Marked; // marked for pruning if previously lost child
        public FibHeapHandle<T> HandleTo;
        internal double Key;
        internal double Rank;

        public FibHeapNode(T Element, double Key)
        {
            this.Element = Element;
            this.Key = Key;
            Children = new HashSet<FibHeapNode<T>>();
            Marked = false;
        }
    }
}
