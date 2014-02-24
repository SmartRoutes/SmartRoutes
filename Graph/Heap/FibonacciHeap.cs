using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRoutes.Graph.Heap
{
    [Serializable()]
    public class EmptyHeapException : Exception
    {
            public EmptyHeapException() : base() { }
            public EmptyHeapException(string message) : base(message) { }
            public EmptyHeapException(string message, System.Exception inner) : base(message, inner) { }

            // A constructor is needed for serialization when an 
            // exception propagates from a remoting server to the client.  
            protected EmptyHeapException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) { }
    }


    public class FibonacciHeap<TValue, TKey> : IFibonacciHeap<TValue, TKey> 
        where TKey : IComparable
    {
        private List<FibHeapNode<TValue, TKey>> Roots;
        private FibHeapNode<TValue, TKey> Min;
        private FibHeapNode<TValue, TKey>[] RootsOfRank; // index i contains root of rank i
        private static int MaxRoots = 1000;

        public FibonacciHeap()
        {
            Roots = new List<FibHeapNode<TValue, TKey>>();
            RootsOfRank = new FibHeapNode<TValue, TKey>[MaxRoots]; // you would need 2^MaxRoots nodes to get root of rank 100

            for (int i = 0; i < MaxRoots; i++) RootsOfRank[i] = null;
        }

        // check for emptiness
        public bool Empty()
        {
            return (Roots.Count() > 0) ? false : true;
        }

        // insert an element
        public FibHeapHandle<TValue, TKey> Insert(TValue Element, TKey Key)
        {
            // create new tree
            var newTree = new FibHeapNode<TValue, TKey>(Element, Key);

            AddToRoot(newTree);

            var handle = new FibHeapHandle<TValue, TKey>(newTree, this);
            newTree.HandleTo = handle;
            return handle;
        }

        // get min element and update heap
        public TValue DeleteMin()
        {
            if (Empty()) throw new EmptyHeapException();

            var ReturnMin = Min;

            // remove min from RootsOfRank array, if it is there
            if (RootsOfRank[Min.Rank] == Min) RootsOfRank[Min.Rank] = null;

            // remove min from roots and merge children of min into roots
            Roots.RemoveAll(x => x == Min);
            foreach (var child in Min.Children.ToArray()) AddToRoot(child);

            // update min
            if (Roots.Count > 0)
            {
                Min = Roots.First();
                foreach (var root in Roots)
                {
                    if (root.Key.CompareTo(Min.Key) < 0) Min = root;
                }
            }
            else
            {
                Min = null;
            }

            if (ReturnMin.HandleTo != null) ReturnMin.HandleTo.ValidHandle = false;
            return ReturnMin.Element;
        }

        public void UpdateKey(FibHeapHandle<TValue, TKey> handle, TKey newKey)
        {
            if (handle.ValidHandle && handle.ParentHeap == this)
            {
                if (newKey.CompareTo(handle.Node.Key) > 0)
                {
                    IncreaseKey(handle.Node, newKey);
                }
                else if (newKey.CompareTo(handle.Node.Key) < 0)
                {
                    DecreaseKey(handle.Node, newKey);
                }
            }
        }

        // merge two trees
        private void Link(FibHeapNode<TValue, TKey> root1, FibHeapNode<TValue, TKey> root2)
        {
            if (root1.Key.CompareTo(root2.Key) > 0)
            {
                Link(root2, root1);
            }
            else
            {
                // tree1's rank will increase, remove reference to it from RootsOfRank array
                if (RootsOfRank[root1.Rank] == root1) RootsOfRank[root1.Rank] = null;

                // tree2 is no longer a root, remove reference to it as well
                if (RootsOfRank[root2.Rank] == root2) RootsOfRank[root2.Rank] = null;

                // remove tree2 from roots
                Roots.RemoveAll(x => x == root2);

                root1.Children.Add(root2);
                root2.Parent = root1;
            }
        }

        // link any root trees of same rank
        private void ConsolidateTrees()
        {
            bool linkNeeded = true;
            while (linkNeeded)
            {
                linkNeeded = InnerConsolidateTrees();
            }
        }

        // having inner function prevents stack from overflowing with too many recursions
        private bool InnerConsolidateTrees()
        {
            bool linkNeeded = false;
            FibHeapNode<TValue, TKey> root1 = null, root2 = null; // store roots to link while enumerating

            foreach(var root in Roots.ToArray())
            {
                if (RootsOfRank[root.Rank] == null)
                {
                    RootsOfRank[root.Rank] = root;
                }
                else if (root != RootsOfRank[root.Rank]) // two roots have same rank
                {
                    root1 = RootsOfRank[root.Rank];
                    root2 = root;
                    linkNeeded = true;
                    break;
                }
            }

            if (linkNeeded)
            {
                Link(root1, root2);
            }

            return linkNeeded;
        }

        // prune the tree
        private void CutOrMark(FibHeapNode<TValue, TKey> Tree)
        {
             // cut tree and it's ancestors
            while (Tree != null)
            {
                Tree = InnerCutOrMark(Tree);
            }
        }

        // cuts tree and returns returns next node to cut, or returns null
        private FibHeapNode<TValue, TKey> InnerCutOrMark(FibHeapNode<TValue, TKey> Tree)
        {
            FibHeapNode<TValue, TKey> ReturnNode = Tree.Parent;

            // don't try to cut root
            if (Tree.Marked == true && Tree.Parent != null)
            {
                if (Tree.Parent.Parent != null)
                {
                    // parent has now lost a child, mark if not a root
                    Tree.Parent.Marked = true; 
                }

                // Tree.Parent rank will change, remove it from RootsOfRank array
                if (RootsOfRank[Tree.Parent.Rank] == Tree.Parent)
                {
                    RootsOfRank[Tree.Parent.Rank] = null;
                }

                Tree.Parent.Children.RemoveAll(x => x == Tree); // cut tree is no longer child of parent
                
                AddToRoot(Tree);
            }
            else if (Tree.Parent != null) // don't mark roots
            {
                Tree.Marked = true;
            }

            return ReturnNode;
        }

        // decrease key value and updates heap
        private void DecreaseKey(FibHeapNode<TValue, TKey> Tree, TKey newKey)
        {
            Tree.Key = newKey;
            Tree.HandleTo.key = newKey;

            if (Tree.Parent == null)
            { 
                // element is a root
                if (Tree != Min && newKey.CompareTo(Min.Key) < 0) Min = Tree;
            }
            else if (Tree.Parent.Key.CompareTo(newKey) <= 0)
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
        private void IncreaseKey(FibHeapNode<TValue, TKey> Tree, TKey newKey)
        {
            Tree.Key = newKey;
            Tree.HandleTo.key = newKey;

            FibHeapNode<TValue, TKey>[] HeapViolators = (from child in Tree.Children
                                where child.Key.CompareTo(newKey) < 0
                                select child).ToArray();

            int childrenLost = 0;

            for (int i = 0; i < HeapViolators.Count(); i++)
            {
                AddToRoot(HeapViolators[i]);
                childrenLost++;
            }

            if (childrenLost > 0)
            {
                if (RootsOfRank[Tree.Rank] == Tree) RootsOfRank[Tree.Rank] = null;
                CutOrMark(Tree);
            }
            if (childrenLost > 1)
            {
                CutOrMark(Tree);
            }
        }

        private void AddToRoot(FibHeapNode<TValue, TKey> Tree)
        {
            Tree.Marked = false;

            if (Tree.Parent != null)
            {
                if (RootsOfRank[Tree.Parent.Rank] == Tree.Parent)
                {
                    RootsOfRank[Tree.Parent.Rank] = null;
                }
                Tree.Parent.Children.RemoveAll(x => x == Tree);
                CutOrMark(Tree.Parent);
                Tree.Parent = null;
            }

            if (Roots.Count() > 0)
            {
                if (Tree.Key.CompareTo(Min.Key) < 0) Min = Tree;
            }
            else
            {
                Min = Tree;
            }

            Roots.Add(Tree);
            ConsolidateTrees();
        }
    }
}
