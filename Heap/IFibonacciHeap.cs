using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public interface IFibonacciHeap<T, K> where K : IComparable
    {
        bool Empty();
        FibHeapHandle<T, K> Insert(T Element, K Key);
        T DeleteMin();
        void UpdateKey(FibHeapHandle<T, K> handle, K newKey);
    }
}