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
        public FibHeapNode<T> Element;
        public IFibonacciHeap<T> ParentHeap;
        public bool ValidHandle;

        public FibHeapHandle(FibHeapNode<T> Element, FibonacciHeap<T> ParentHeap)
        {
            this.Element = Element;
            this.ParentHeap = ParentHeap;
            ValidHandle = true;
        }
    }
}
