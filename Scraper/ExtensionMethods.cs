using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    public static class ExtensionMethods
    {
        public static string GetSha256Hash(this byte[] bytes)
        {
            var sha = new SHA256Managed();
            byte[] hashBytes = sha.ComputeHash(bytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }
    }
}
