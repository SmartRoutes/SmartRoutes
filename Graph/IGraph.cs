using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;
using SmartRoutes.Heap;

namespace SmartRoutes.Graph
{
    public interface IGraph
    {
        void GetSortaEntities();
        void GetChildCares();
        NodeInfo Dijkstras(ISet<INode> StartNodes, Func<INode, bool> GoalCheck, Direction direction);
        INode[] GraphNodes { get; }
    }
}