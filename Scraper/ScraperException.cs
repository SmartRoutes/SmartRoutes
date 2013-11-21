using System;

namespace SmartRoutes.Scraper
{
    public class ScraperException : Exception
    {
        public ScraperException(string message) : base(message)
        {
        }
    }
}