using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Graph.Heap;

namespace SmartRoutes.Graph.Test
{
    [TestClass]
    public class FibonacciHeapTest
    {
        [TestMethod]
        public void UpdateKey_Single()
        {
            // ARRANGE
            IFibonacciHeap<string, string> heap = new FibonacciHeap<string, string>();
            heap.Insert("a", "mmm_a");
            heap.Insert("c", "mmm_c");
            FibHeapHandle<string, string> handle = heap.Insert("b", "zzz_b");

            // ACT
            heap.UpdateKey(handle, "mmm_b");

            // ASSERT
            Assert.AreEqual("a", heap.DeleteMin());
            Assert.AreEqual("b", heap.DeleteMin());
            Assert.AreEqual("c", heap.DeleteMin());
            Assert.IsTrue(heap.Empty());
        }

        [TestMethod]
        public void UpdateKey_Multiple()
        {
            // ARRANGE
            IFibonacciHeap<string, string> heap = new FibonacciHeap<string, string>();
            heap.Insert("a", "mmm_a");
            heap.Insert("c", "mmm_c");
            FibHeapHandle<string, string> handleD = heap.Insert("d", "aaa_d");
            FibHeapHandle<string, string> handleB = heap.Insert("b", "zzz_b");
            

            // ACT
            heap.UpdateKey(handleD, "mmm_d");
            heap.UpdateKey(handleB, "mmm_b");

            // ASSERT
            Assert.AreEqual("a", heap.DeleteMin());
            Assert.AreEqual("b", heap.DeleteMin());
            Assert.AreEqual("c", heap.DeleteMin());
            Assert.AreEqual("d", heap.DeleteMin());
            Assert.IsTrue(heap.Empty());
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyHeapException))]
        public void DeleteMin_WithEmptyHeap()
        {
            // ARRANGE
            IFibonacciHeap<int, int> heap = new FibonacciHeap<int, int>();

            // ACT
            heap.DeleteMin();
        }

        [TestMethod]
        public void Empty_True()
        {
            // ARRANGE
            IFibonacciHeap<int, int> heap = new FibonacciHeap<int, int>();

            // ACT and ASSERT
            Assert.IsTrue(heap.Empty());
        }

        [TestMethod]
        public void Empty_False()
        {
            // ARRANGE
            IFibonacciHeap<int, int> heap = new FibonacciHeap<int, int>();
            heap.Insert(0, 0);

            // ACT and ASSERT
            Assert.IsFalse(heap.Empty());
        }

        [TestMethod]
        public void InsertAndDeleteMin_HappyPath()
        {
            VerifyInsertAndDeleteMin(new[] {85, 93, 86, 12, 42, 30, 60, 19, 11, 80});
        }

        [TestMethod]
        public void InsertAndDeleteMin_DuplicateValueSameKey()
        {
            var duplicateValue = new object();
            VerifyInsertAndDeleteMin(new[]
            {
                new KeyValuePair<int, object>(4, duplicateValue),
                new KeyValuePair<int, object>(6, new object()),
                new KeyValuePair<int, object>(4, duplicateValue),
                new KeyValuePair<int, object>(2, new object()),
                new KeyValuePair<int, object>(9, new object())
            });
        }

        [TestMethod]
        public void InsertAndDeleteMin_DuplicateValueDifferentKey()
        {
            var duplicateValue = new object();
            VerifyInsertAndDeleteMin(new[]
            {
                new KeyValuePair<int, object>(4, duplicateValue),
                new KeyValuePair<int, object>(6, new object()),
                new KeyValuePair<int, object>(8, duplicateValue),
                new KeyValuePair<int, object>(2, new object()),
                new KeyValuePair<int, object>(9, new object())
            });
        }

        [TestMethod]
        public void InsertAndDeleteMin_SingleItem()
        {
            VerifyInsertAndDeleteMin(new[] {23});
        }

        [TestMethod]
        public void InsertAndDeleteMin_AlreadySorted()
        {
            VerifyInsertAndDeleteMin(new[] {1, 2, 3, 4, 5});
        }

        [TestMethod]
        public void InsertAndDeleteMin_Empty()
        {
            VerifyInsertAndDeleteMin(new int[0]);
        }

        [TestMethod]
        public void InsertAndDeleteMin_ReverseSorted()
        {
            VerifyInsertAndDeleteMin(new[] {5, 4, 3, 2, 1});
        }

        [TestMethod]
        public void InsertAndDeleteMin_DuplicateKeys()
        {
            VerifyInsertAndDeleteMin(new[] {1, 3, 6, 3, 5, 6});
        }

        [TestMethod]
        public void InsertAndDeleteMin_StableSort()
        {
            VerifyInsertAndDeleteMin(new[] {5, 1, 1, 1, 3});
        }

        [TestMethod, Timeout(1000)]
        public void InfiniteLoop()
        {
            // ARRANGE
            var heap = new FibonacciHeap<string, string>();
            var handleR = heap.Insert("R", "R");
            var handleT = heap.Insert("T", "T");
            
            // ACT
            heap.UpdateKey(handleT, "R");
            heap.DeleteMin();
            heap.DeleteMin();

            // ASSERT
            Assert.IsTrue(heap.Empty());
        }

        [TestMethod]
        public void IllegalLink()
        {
            // ARRANGE
            var heap = new FibonacciHeap<int, int>();
            int count = 4;
            FibHeapHandle<int, int>[] handles = new FibHeapHandle<int, int>[count];
            for (int i = 0; i < count; i++)
            {
                handles[i] = heap.Insert(i, i);
            }

            // ACT
            var rand = new Random();
            for (int i = 0; i < count; i++) heap.UpdateKey(handles[i], 0);
            for (int i = 0; i < count; i++) heap.DeleteMin();

            // ASSERT
            Assert.IsTrue(heap.Empty());
        }

        private void VerifyInsertAndDeleteMin<TKey>(IEnumerable<TKey> keys)
            where TKey : IComparable
        {
            VerifyInsertAndDeleteMin(keys
                .Select(k => new KeyValuePair<TKey, object>(k, new object()))
                .ToArray());
        }

        private void VerifyInsertAndDeleteMin<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
            where TKey : IComparable
            where TValue : class
        {
            // enumerate all of the items
            KeyValuePair<TKey, TValue>[] itemArray = items.ToArray();

            // insert all of the items
            IFibonacciHeap<TValue, TKey> heap = new FibonacciHeap<TValue, TKey>();
            foreach (var pair in itemArray)
            {
                heap.Insert(pair.Value, pair.Key);
            }

            // sort the items
            TValue[] expectedValueArray = itemArray
                .OrderBy(p => p.Key)
                .Select(p => p.Value)
                .ToArray();

            // pop all of the items
            IList<TValue> actualValueList = new List<TValue>();
            while (!heap.Empty())
            {
                actualValueList.Add(heap.DeleteMin());
            }

            // verify the ordered items
            Assert.AreEqual(expectedValueArray.Length, actualValueList.Count);
            for (int i = 0; i < expectedValueArray.Length; i++)
            {
                Assert.AreSame(expectedValueArray[i], actualValueList[i]);
            }
        }
    }
}