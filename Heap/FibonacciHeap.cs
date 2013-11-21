using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutes.Heap
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

        public FibonacciHeap()
        {
            Roots = new List<FibHeapNode<TValue, TKey>>();
            RootsOfRank = new FibHeapNode<TValue,TKey>[1000]; // you would need a LOT of nodes to get tree of rank 1000

            for (int i = 0; i < 1000; i++) RootsOfRank[i] = null;
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

            ConsolidateTrees();

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

        public void UpdateKey(FibHeapHandle<TValue, TKey> handle, TKey newKey)
        {
            if (handle.ValidHandle && handle.ParentHeap == this)
            {
                if (newKey.CompareTo(handle.Node.Key) < 0) DecreaseKey(handle.Node, newKey);
                else IncreaseKey(handle.Node, newKey);
            }
        }

        // merge two trees
        private void Link(FibHeapNode<TValue, TKey> tree1, FibHeapNode<TValue, TKey> tree2)
        {
            if (tree1.Key.CompareTo(tree2.Key) > 0)
            {
                Link(tree2, tree1);
            }
            else
            {
                // tree1's rank will increase, remove reference to it from RootsOfRank array
                if (RootsOfRank[tree1.Rank] == tree1) RootsOfRank[tree1.Rank] = null;

                // tree2 is no longer a root, remove reference to it as well
                if (RootsOfRank[tree2.Rank] == tree2) RootsOfRank[tree2.Rank] = null;

                tree1.Children.Add(tree2);
                tree2.Parent = tree1;
                Roots.RemoveAll(x => x == tree2);
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
            FibHeapNode<TValue, TKey> tree1 = null, tree2 = null; // store roots to link while enumerating

            var RootsEnum = Roots.GetEnumerator();

            while (RootsEnum.MoveNext() && !linkNeeded)
            {
                var root = RootsEnum.Current;

                if (RootsOfRank[root.Rank] == null)
                {
                    RootsOfRank[root.Rank] = root;
                }
                else if (root != RootsOfRank[root.Rank]) // two roots have same rank
                {
                    linkNeeded = true;

                    if (root.Key.CompareTo(RootsOfRank[root.Rank].Key) < 0)
                    {
                        tree1 = root;
                        tree2 = RootsOfRank[root.Rank];
                    }
                    else
                    {
                        tree1 = RootsOfRank[root.Rank];
                        tree2 = root;
                    }
                }
            }

            if (linkNeeded)
            {
                Link(tree1, tree2);
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
            FibHeapNode<TValue, TKey> ReturnNode = null;

            // don't try to cut root
            if (Tree.Marked == true && Tree.Parent != null)
            {
                ReturnNode = Tree.Parent;

                Tree.Parent.Marked = true; // parent has now lost a child

                // Tree.Parent rank will chang, remove it from RootsOfRank array
                if (RootsOfRank[Tree.Parent.Rank] == Tree.Parent)
                {
                    RootsOfRank[Tree.Parent.Rank] = null;
                }

                Tree.Parent.Children.RemoveAll(x => x == Tree); // cut tree is no longer child of parent
                Tree.Parent = null; // cut tree will no longer have parent
                Tree.Marked = false; // cut Tree no longer marked

                AddToRoot(Tree);
            }
            else
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
        private void IncreaseKey(FibHeapNode<TValue, TKey> Tree, TKey newKey)
        {
            Tree.Key = newKey;
            Tree.HandleTo.key = newKey;

            FibHeapNode<TValue, TKey>[] HeapViolators = (from child in Tree.Children
                                where child.Key.CompareTo(newKey) < 0
                                select child).ToArray();

            // if rank of Tree will change, remove it from RootsOfRank array
            if (HeapViolators.Count() > 0 && RootsOfRank[Tree.Rank] == Tree)
            {
                RootsOfRank[Tree.Rank] = null;
            }

            foreach (var child in HeapViolators)
            {
                Tree.Children.RemoveAll(x => x == child);
                if (child.Key.CompareTo(Min.Key) < 0) Min = child;
                AddToRoot(child);
                CutOrMark(Tree);
            }

            ConsolidateTrees();
        }

        private void AddToRoot(FibHeapNode<TValue, TKey> Tree)
        {
            Tree.Marked = false;
            Tree.Parent = null;

            if (RootsOfRank[Tree.Rank] == null) RootsOfRank[Tree.Rank] = Tree;

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
