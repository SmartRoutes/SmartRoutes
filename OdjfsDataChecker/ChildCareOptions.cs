using CommandLine;

namespace OdjfsDataChecker
{
    public class ChildCareOptions
    {
        [Option('u',
            Required = true,
            HelpText = "The URL ID of a specific child care to scrape, e.g. \"CDCSFJQMQINKNININI\".")]
        public string ExternalUrlId { get; set; }
    }
}