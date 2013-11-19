using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    internal class FibHeapNode<TValue, TKey> where TKey : IComparable
    {
        internal readonly TValue Element;
        internal List<FibHeapNode<TValue, TKey>> Children;
        internal FibHeapNode<TValue, TKey> Parent;
        internal bool Marked; // marked for pruning if previously lost child
        internal FibHeapHandle<TValue, TKey> HandleTo;
        internal TKey Key;

        internal FibHeapNode(TValue Element, TKey Key)
        {
            this.Element = Element;
            this.Key = Key;
            Children = new List<FibHeapNode<TValue, TKey>>();
            Marked = false;
        }

         // number of children
        internal long Rank
        {
            get { return Children.Count(); }
        }
    }
}
