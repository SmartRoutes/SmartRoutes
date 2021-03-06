﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoutes.Graph
{
    public struct GraphBuilderSettings
    {
        public double MaxFeetFromDestinationToStop;
        public int MaxDestinationCloseStops;
        public double WalkingFeetPerSecond;
        public double MaxFeetBetweenTransfers;

        public static GraphBuilderSettings Default
        {
            get
            {
                return new GraphBuilderSettings
                {
                    MaxFeetFromDestinationToStop = 5000,
                    MaxDestinationCloseStops = 5,
                    WalkingFeetPerSecond = 4.4,
                    MaxFeetBetweenTransfers = 1500
                };
            }
        }
    }
}
