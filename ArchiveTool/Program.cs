using Ninject;
using Ninject.Extensions.Conventions;
using SmartRoutes.ArchiveLoader;
using SmartRoutes.Model.Gtfs;

namespace SmartRoutes.ArchiveTool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind(c => c
                .From("SmartRoutes.Reader.dll", "SmartRoutes.ArchiveLoader.dll")
                .SelectAllClasses()
                .BindAllInterfaces());

            var loader = kernel.Get<IArchiveLoader<GtfsArchive, GtfsCollection>>();
            loader.Read(@"D:\Dropbox\School\Spring 2013-2014\Senior Design II\SORTA\google_transit_info.zip", true).Wait();
        }
    }
}