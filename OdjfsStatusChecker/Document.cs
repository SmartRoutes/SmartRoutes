using System;
using System.IO;

namespace SmartRoutes.OdjfsStatusChecker
{
    [Serializable]
    public class Document
    {
        public string RequestUri { get; set; }
        public string Name { get; set; }

        public string GetPath(string directory, string identifier, string extension)
        {
            string name = string.Format(
                "{0}_{1}.{2}",
                Name,
                identifier,
                extension);

            return Path.Combine(directory, name);
        }
    }
}