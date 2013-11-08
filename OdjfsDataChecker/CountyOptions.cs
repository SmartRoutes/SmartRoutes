﻿using CommandLine;

namespace OdjfsDataChecker
{
    public class CountyOptions
    {
        [Option('n',
            Required = true,
            HelpText = "The name of the county to scrape, e.g. \"Hamilton\".")]
        public string Name { get; set; }
    }
}