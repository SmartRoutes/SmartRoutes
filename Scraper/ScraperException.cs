using System;

namespace Scraper
{
    public class ScraperException : Exception
    {
        public ScraperException(string message) : base(message)
        {
        }
    }
}