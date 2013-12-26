using System;
using NDesk.Options;
using Ninject;
using Ninject.Extensions.Conventions;
using NLog;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Readers;

namespace SmartRoutes.SortaDataChecker
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            // parse the command line arguments
            string archivePath = null;
            bool force = false;
            bool showHelp = false;
            var optionSet = new OptionSet
            {
                {"a|archive=", "path to a SORTA data archive (.zip file)", v => archivePath = v},
                {"f|force", "force the SORTA data to be loaded", v => force = true},
                {"h|?|help", "show this message", v => showHelp = true}
            };

            try
            {
                optionSet.Parse(args);

                // show help, if necessary
                if (showHelp)
                {
                    ShowHelp(optionSet);
                    return;
                }
            }
            catch (OptionException e)
            {
                Console.WriteLine("{0}:");
                Console.WriteLine();
                Console.WriteLine("Try `{0} --help` for more information.", AppDomain.CurrentDomain.FriendlyName);
                return;
            }

            try
            {
                Logger.Trace("SortaDataChecker is now starting.");

                IKernel kernel = new StandardKernel();
                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof (GtfsCollection))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                DataChecker dataChecker;
                if (archivePath != null)
                {
                    var reader = kernel.Get<IEntityCollectionReader<GtfsArchive, GtfsCollection>>();
                    dataChecker = new DataChecker(reader, archivePath);
                }
                else
                {
                    var downloader = kernel.Get<IEntityCollectionDownloader<GtfsArchive, GtfsCollection>>();
                    dataChecker = new DataChecker(downloader, new Uri("http://www.go-metro.com/uploads/GTFS/google_transit_info.zip"));
                }

                dataChecker.UpdateDatabase(force).Wait();

                Logger.Trace("SortaDataChecker has completed.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("An exception has forced the SortaDataChecker to terminate.", e);
                throw;
            }
        }

        private static void ShowHelp(OptionSet optionSet)
        {
            Console.WriteLine("Usage: {0} [OPTIONS]", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("  Used to update your local machine's SORTA database.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            optionSet.WriteOptionDescriptions(Console.Out);
        }
    }
}