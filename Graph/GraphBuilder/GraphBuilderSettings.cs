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
        public int MaxDestinationCloseStops;
        public double WalkingFeetPerSecond;
        public double MaxFeetBetweenTransfers;

        public static GraphBuilderSettings Default
        {
            get
            {
                return new GraphBuilderSettings
                {
                    MaxFeetFromChildCareToBuStop = 5000,
                    MaxDestinationCloseStops = 5,
                    WalkingFeetPerSecond = 2.2,
                    MaxFeetBetweenTransfers = 500
                };
            }
        }
    }
}
