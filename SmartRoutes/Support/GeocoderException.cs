using System;

namespace SmartRoutes.Support
{
    public class GeocoderException : Exception
    {
        public GeocoderException(string message) : base(message)
        {
        }
    }
}