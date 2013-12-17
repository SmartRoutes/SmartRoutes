using System.Text;

namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public abstract class BaseTemplate
    {
        public static byte[] GetBytes(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }
    }
}