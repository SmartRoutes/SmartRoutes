using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.Graph.Node
{
    public class ChildCareNode : IChildcareNode
    {
        public DateTime Time { get; private set; }
        public NodeBase BaseNode { get; private set; }
        public ISet<INode> UpwindNeighbors { get; private set; }
        public ISet<INode> DownwindNeighbors { get; private set; }

        // legacy properties
        public string Name { get { return BaseNode.Name; } }
        public double Latitude { get { return BaseNode.Latitude; } }
        public double Longitude { get { return BaseNode.Longitude; } }

        public ChildCareNode(ChildCare childcare, DateTime Time, NodeBase BaseNode)
        {
            this.BaseNode = BaseNode;
            this.Time = Time;
            UpwindNeighbors = new HashSet<INode>();
            DownwindNeighbors = new HashSet<INode>();
        }
    }
}
