using System;
using System.IO;
using ManyConsole;
using SmartRoutes.ArchiveLoader;
using SmartRoutes.Model;

namespace SmartRoutes.ArchiveTool.Commands
{
    public abstract class ArchiveCommand<TArchive, TCollection> : ConsoleCommand, IArchiveCommand
        where TArchive : Archive
        where TCollection : EntityCollection<TArchive>
    {
        private readonly IArchiveLoader<TArchive, TCollection> _archiveLoader;

        protected ArchiveCommand(string archiveName, IArchiveLoader<TArchive, TCollection> archiveLoader)
        {
            string archiveNameUpper = archiveName.ToUpper();
            string archiveNameLower = archiveName.ToLower();

            _archiveLoader = archiveLoader;

            IsCommand(archiveNameLower, string.Format("load an {0} archive to the configured database", archiveNameUpper));
            HasOption("p|path=", string.Format("load a {0} archive from a file path", archiveNameUpper), v => FilePath = v);
            HasOption("u|url=", string.Format("load a {0} archive from a URL", archiveNameUpper), v => Url = new Uri(v));
            HasOption("f|force", string.Format("force the {0} archive to be loaded", archiveNameUpper), v => Force = true);
        }

        public string FilePath { get; set; }
        public Uri Url { get; set; }
        public bool Force { get; set; }

        public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
        {
            base.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments);

            if (Url != null && FilePath != null)
            {
                throw new ConsoleHelpAsException("The -p/--path and -u/--url are mutually exclusive.");
            }

            if (Url == null && FilePath == null)
            {
                throw new ConsoleHelpAsException("Either the -p/--path or -u/--url argument must be specified.");
            }

            if (!File.Exists(FilePath))
            {
                throw new ConsoleHelpAsException("The provided -p/--path does not exist.");
            }

            return null;
        }

        public override int Run(string[] remainingArguments)
        {
            if (FilePath != null)
            {
                _archiveLoader.Read(FilePath, Force).Wait();
            }
            else
            {
                _archiveLoader.Download(Url, Force).Wait();
            }

            return 0;
        }
    }
}