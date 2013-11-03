using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public interface IFibonacciHeap<T>
    {
        bool Empty();
        FibHeapHandle<T> Insert(T Element, double Key);
        T DeleteMin();
        void UpdateKey(FibHeapHandle<T> handle, double newKey);
    }
}