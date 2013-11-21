using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutes.Heap
{
    // provide handles to inserted objects
    public class FibHeapHandle<TValue, TKey> where TKey : IComparable
    {
        internal readonly FibHeapNode<TValue, TKey> Node;
        internal readonly IFibonacciHeap<TValue, TKey> ParentHeap;
        internal bool ValidHandle;
        public readonly TValue value;
        internal TKey key;

        internal FibHeapHandle(FibHeapNode<TValue, TKey> Element, FibonacciHeap<TValue, TKey> ParentHeap)
        {
            value = Element.Element;
            key = Element.Key;
            this.Node = Element;
            this.ParentHeap = ParentHeap;
            ValidHandle = true;
        }
    }
}
