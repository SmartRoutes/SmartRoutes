using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    // provide handles to inserted objects
    public class FibHeapHandle<T, K> where K : IComparable
    {
        internal readonly FibHeapNode<T, K> Element;
        internal readonly IFibonacciHeap<T, K> ParentHeap;
        internal bool ValidHandle;

        internal FibHeapHandle(FibHeapNode<T, K> Element, FibonacciHeap<T, K> ParentHeap)
        {
            this.Element = Element;
            this.ParentHeap = ParentHeap;
            ValidHandle = true;
        }
    }
}
