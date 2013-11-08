using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heap;
using Graph.Node;

namespace Graph
{
    public interface IGraph
    {
        IFibonacciHeap<INode, TimeSpan> Queue { get; set; }
        void updateSortaEntities();
        NodeInfo Dijkstras(ISet<INode> StartNodes, Func<INode, bool> GoalCheck, Direction direction);
        INode[] GraphNodes { get; }
    }
}