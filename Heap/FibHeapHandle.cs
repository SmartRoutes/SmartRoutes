using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    // provide handles to inserted objects
    public class FibHeapHandle<T>
    {
        internal readonly FibHeapNode<T> Element;
        internal readonly IFibonacciHeap<T> ParentHeap;
        internal bool ValidHandle;

        internal FibHeapHandle(FibHeapNode<T> Element, FibonacciHeap<T> ParentHeap)
        {
            this.Element = Element;
            this.ParentHeap = ParentHeap;
            ValidHandle = true;
        }
    }
}
