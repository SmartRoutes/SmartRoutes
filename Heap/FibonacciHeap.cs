using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public class FibonacciHeap<T, K> : IFibonacciHeap<T, K> where K : IComparable
    {
        private List<FibHeapNode<T, K>> Roots;
        private FibHeapNode<T, K> Min;

        public FibonacciHeap()
        {
            Roots = new List<FibHeapNode<T, K>>();
        }

        // check for emptiness
        public bool Empty()
        {
            return (Roots.Count() > 0) ? false : true;
        }

        // insert an element
        public FibHeapHandle<T, K> Insert(T Element, K Key)
        {
            // create new tree
            var newTree = new FibHeapNode<T, K>(Element, Key);

            AddToRoot(newTree);
            ConsolidateTrees();

            var handle = new FibHeapHandle<T, K>(newTree, this);
            newTree.HandleTo = handle;
            return handle;
        }

        // get min element and update heap
        public T DeleteMin()
        {
            var ReturnMin = Min;

            // remove min from roots and merge children of min into roots
            Roots.Remove(Min);
            foreach (var child in Min.Children) AddToRoot(child);
            ConsolidateTrees();

            // update min
            if (Roots.Count > 0)
            {
                Min = Roots.First();
                foreach (var root in Roots)
                {
                    if (root.Key.CompareTo(Min.Key) < 0) Min = root;
                }
            }

            if (ReturnMin.HandleTo != null) ReturnMin.HandleTo.ValidHandle = false;
            return ReturnMin.Element;
        }

        public void UpdateKey(FibHeapHandle<T, K> handle, K newKey)
        {
            if (handle.ValidHandle && handle.ParentHeap == this)
            {
                if (newKey.CompareTo(handle.Node.Key) < 0) DecreaseKey(handle.Node, newKey);
                else IncreaseKey(handle.Node, newKey);
            }
        }

        // merge two trees
        private void Link(FibHeapNode<T, K> tree1, FibHeapNode<T, K> tree2)
        {
            if (tree1.Key.CompareTo(tree2.Key) > 0)
            {
                Link(tree2, tree1);
            }
            else
            {
                tree1.Rank += tree2.Rank + 1;
                tree1.Children.Add(tree2);
                tree2.Parent = tree1;
            }
        }

        // link any root trees of same rank
        private void ConsolidateTrees()
        {
            while (InnerConsolidateTrees()) ;
        }

        // having inner function prevents stack from overflowing with too many recursions
        private bool InnerConsolidateTrees()
        {
            bool linkNeeded = false;

            var enumerator1 = Roots.GetEnumerator();
            FibHeapNode<T, K> tree1 = null, tree2 = null;

            while (enumerator1.MoveNext() && !linkNeeded)
            {
                var enumerator2 = enumerator1;

                while (enumerator2.MoveNext() && !linkNeeded)
                {
                    if (enumerator1.Current.Rank == enumerator2.Current.Rank)
                    {
                        linkNeeded = true;
                        if (enumerator1.Current.Key.CompareTo(enumerator2.Current.Key) > 0)
                        {
                            tree1 = enumerator2.Current;
                            tree2 = enumerator1.Current;
                        }
                        else
                        {
                            tree1 = enumerator1.Current;
                            tree2 = enumerator2.Current;
                        }
                    }
                }
            }

            if (linkNeeded)
            {
                Roots.Remove(tree2);
                Link(tree1, tree2);
            }

            return linkNeeded;
        }

        // prune the tree
        private void CutOrMark(FibHeapNode<T, K> Tree)
        {
             // cut tree and it's ancestors
            while (Tree != null)
            {
                Tree = InnerCutOrMark(Tree);
            }
        }

        // cuts tree and returns returns next node to cut, or returns null
        private FibHeapNode<T, K> InnerCutOrMark(FibHeapNode<T, K> Tree)
        {
            FibHeapNode<T, K> ReturnNode = null;

            // don't try to cut root
            if (Tree.Marked == true && Tree.Parent != null)
            {
                ReturnNode = Tree.Parent;

                Tree.Parent.Marked = true; // parent has now lost a child
                Tree.Parent.Children.Remove(Tree); // cut tree is no longer child of parent
                Tree.Parent.Rank -= Tree.Rank + 1; // parent has lost Tree.Rank + 1 children 
                Tree.Parent = null; // cut tree will no longer have parent

                AddToRoot(Tree);
            }

            Tree.Marked = false;
            return ReturnNode;
        }

        // decrease key value and updates heap
        private void DecreaseKey(FibHeapNode<T, K> Tree, K newKey)
        {
            Tree.Key = newKey;
            Tree.HandleTo.key = newKey;

            if (Tree.Parent == null)
            { 
                // element is a root
                if (newKey.CompareTo(Min.Key) < 0) Min = Tree;
            }
            else if (Tree.Parent.Key.CompareTo(newKey) < 0)
            { 
                // heap order not violated
            }
            else
            { 
                // heap order violated
                Tree.Marked = true;
                CutOrMark(Tree);
            }
        }

        // increases key value and updates heap
        private void IncreaseKey(FibHeapNode<T, K> Tree, K newKey)
        {
            Tree.Key = newKey;
            Tree.HandleTo.key = newKey;

            FibHeapNode<T, K>[] HeapViolators = (from child in Tree.Children
                                where child.Key.CompareTo(newKey) < 0
                                select child).ToArray();

            foreach (var child in HeapViolators)
            {
                Tree.Children.Remove(child);
                Tree.Rank -= child.Rank - 1;
                if (child.Key.CompareTo(Min.Key) < 0) Min = child;
                AddToRoot(child);
                CutOrMark(Tree);
            }

            ConsolidateTrees();
        }

        private void AddToRoot(FibHeapNode<T, K> Tree)
        {
            Tree.Marked = false;
            Tree.Parent = null;
            
            if (Roots.Count() > 0)
            {
                if (Tree.Key.CompareTo(Min.Key) < 0) Min = Tree;
            }
            else
            {
                Min = Tree;
            }

            Roots.Add(Tree);
        }
    }
}
