using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public class FibHeapNode<T>
    {
            public T Element;
            public double Key;
            public ISet<FibHeapNode<T>> Children;
            public FibHeapNode<T> Parent;
            public long Rank;
            public bool Marked; // marked for pruning if previously lost child
            public FibHeapHandle<T> HandleTo;

            public FibHeapNode(T Element, double Key)
            {
                this.Element = Element;
                this.Key = Key;
                Children = new HashSet<FibHeapNode<T>>();
                Rank = 0;
                Marked = false;
            }
    }
}
