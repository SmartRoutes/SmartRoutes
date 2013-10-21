using System;
using System.Security.Cryptography;

namespace OdjfsHtmlScraper.Support
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