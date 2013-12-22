using System;
using System.Reflection;
using System.Security.Cryptography;

namespace SmartRoutes.Reader
{
    public static class ExtensionMethods
    {
        public static string GetSha256Hash(this byte[] bytes)
        {
            var sha = new SHA256Managed();
            byte[] hashBytes = sha.ComputeHash(bytes);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public static string GetInformationalVersion(this Assembly assembly)
        {
            return ((AssemblyInformationalVersionAttribute) assembly
                .GetCustomAttributes(typeof (AssemblyInformationalVersionAttribute), false)[0])
                .InformationalVersion;
        }
    }
}