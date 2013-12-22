using System;

namespace SmartRoutes.Reader
{
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message)
        {
        }
    }
}