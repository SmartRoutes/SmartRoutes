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
        internal readonly FibHeapNode<T, K> Node;
        internal readonly IFibonacciHeap<T, K> ParentHeap;
        internal bool ValidHandle;
        public readonly T value;
        internal K key;

        internal FibHeapHandle(FibHeapNode<T, K> Element, FibonacciHeap<T, K> ParentHeap)
        {
            value = Element.Element;
            key = Element.Key;
            this.Node = Element;
            this.ParentHeap = ParentHeap;
            ValidHandle = true;
        }
    }
}
