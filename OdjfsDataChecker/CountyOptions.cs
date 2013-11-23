﻿using CommandLine;

namespace SmartRoutes.OdjfsDataChecker
{
    public class CountyOptions
    {
        [Option('n',
            HelpText = "The name of the county to scrape, e.g. \"Hamilton\".")]
        public string Name { get; set; }
    }
}