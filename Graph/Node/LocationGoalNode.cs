using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutes.Graph.Node
{
    class LocationGoalNode : INode
    {
        public LocationGoalNode(DateTime Time)
        {
            BaseNode = new NodeBase("", 0, 0);
            this.Time = Time;
        }

        public NodeBase BaseNode { get; private set; }
        public DateTime Time { get; private set; }
        public ISet<INode> TimeBackwardNeighbors { get { throw new NotImplementedException(); } }
        public ISet<INode> TimeForwardNeighbors { get { throw new NotImplementedException(); } }
        public string Name { get { return "Goal"; } }
        public double Latitude { get { throw new NotImplementedException(); } }
        public double Longitude { get { throw new NotImplementedException(); } }
    }
}
