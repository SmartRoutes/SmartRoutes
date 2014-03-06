using System.Security.Cryptography.X509Certificates;
using SmartRoutes.Model;

namespace SmartRoutes.Graph.Node
{
    public interface IDestinationNode : INode
    {
        IDestination Destination { get; } 
    }
}