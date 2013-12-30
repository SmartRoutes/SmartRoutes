using System;

namespace SmartRoutes.Reader.Support
{
    public class ScraperException : Exception
    {
        public ScraperException(string message) : base(message)
        {
        }
    }
}