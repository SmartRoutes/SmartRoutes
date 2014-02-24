using System;

namespace SmartRoutes.Graph.Heap
{
    public interface IFibonacciHeap<TValue, TKey> where TKey : IComparable
    {
        bool Empty();
        FibHeapHandle<TValue, TKey> Insert(TValue Element, TKey Key);
        TValue DeleteMin();
        void UpdateKey(FibHeapHandle<TValue, TKey> handle, TKey newKey);
    }
}