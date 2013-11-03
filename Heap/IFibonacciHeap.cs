using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public interface IFibonacciHeap<T>
    {
        public bool Empty();
        public FibonacciHeap<T>.FibHeapHandle Insert(T Element, double Key);
        public T DeleteMin();
    }
}
