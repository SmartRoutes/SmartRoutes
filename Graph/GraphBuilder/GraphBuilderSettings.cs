using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutes.Graph
{
    public struct GraphBuilderSettings
    {
        public double MaxFeetFromChildCareToBuStop;
        public int MaxChildCareCloseStops;
        public double WalkingFeetPerSecond;

        public static GraphBuilderSettings Default
        {
            get
            {
                return new GraphBuilderSettings
                {
                    MaxFeetFromChildCareToBuStop = 1000000,
                    MaxChildCareCloseStops = 5,
                    WalkingFeetPerSecond = 1.5
                };
            }
        }
    }
}
