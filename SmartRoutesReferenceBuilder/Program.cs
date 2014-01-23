using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace SmartRoutesReferenceBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string scriptsDirectory = args[0];
                string referencesFileName = "_references.js";
                if (Directory.Exists(scriptsDirectory))
                {
                    string[] files = Directory.GetFiles(scriptsDirectory, "*.*", SearchOption.AllDirectories);

                    StringBuilder referenceBuilder = new StringBuilder();
                    foreach (string filePath in files)
                    {
                        // Explicitly exclude this file.
                        if (string.Compare(referencesFileName, Path.GetFileName(filePath), true) != 0)
                        {
                            if (filePath.EndsWith(".js"))
                            {
                                string relativePath = filePath.Remove(0, scriptsDirectory.Length);
                                referenceBuilder.AppendFormat("/// <reference path=\"{0}\" />", relativePath);
                                referenceBuilder.AppendLine();
                            }
                        }
                    }

                    using (StreamWriter referenceWriter = new StreamWriter(Path.Combine(scriptsDirectory, referencesFileName), false))
                    {
                        referenceWriter.Write(referenceBuilder.ToString());
                    }
                }
            }
        }
    }
}
