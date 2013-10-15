using System;

namespace Scraper
{
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}