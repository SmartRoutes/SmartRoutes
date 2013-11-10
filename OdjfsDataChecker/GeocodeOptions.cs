using CommandLine;

namespace OdjfsDataChecker
{
    public class GeocodeOptions
    {
        [Option('u',
            HelpText = "The URL ID of a specific child care to geocode, e.g. \"CDCSFJQMQINKNININI\".")]
        public string ExternalUrlId { get; set; }
    }
}