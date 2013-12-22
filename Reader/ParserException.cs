using System;

namespace SmartRoutes.Scraper
{
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}