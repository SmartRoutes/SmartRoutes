using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Graph.Node;

namespace SmartRoutes.Graph
{
    public class SearchKeyManager
    {
        public Func<INode, bool>[] CriteriaArray;

        public SearchKeyManager(IEnumerable<Func<INode, bool>> Criteria)
        {
            CriteriaArray = Criteria.ToArray();
        }

        // returns search key for a node based on that node's parent
        public Tuple<NodeBase, string> neighborKey(INode node, NodeInfo parentInfo)
        {
            var indexStrings = parentInfo.UnsatisfiedCriteria.Split('-');
            var indeces = indexStrings.Where(str => str != "").Select(str => Convert.ToInt32(str));

            string filteredStr = "";
            foreach (var i in indeces)
            {
                if (!CriteriaArray[i](node))
                {
                    filteredStr = string.Concat(new[] { filteredStr, i.ToString(), "-" });
                }
            }

            return new Tuple<NodeBase, string>(node.BaseNode, filteredStr);
        }

        // creates string representing unsatisfied criteria for a given node
        public string UnsatisfiedCriteria(INode node)
        {
            string str = "";

            for (int i = 0; i < CriteriaArray.Count(); i++)
            {
                if (!CriteriaArray[i](node))
                {
                    str = string.Concat(new[] { str, i.ToString(), "-" });
                }
            }

            return str;
        }
    }
}
