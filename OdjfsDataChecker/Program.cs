using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Database.Contexts;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Parameters;
using NLog;
using OdjfsScraper.Scrapers;
using OdjfsScraper.Support;

namespace OdjfsDataChecker
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            const int maximumDuration = 5*60*1000; // five minutes, in milliseconds
            const int sleepDuration = 3*1000; // three seconds, in milliseconds

            // start timing
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // update a list and sleep (if possible)
            Update(true);
            if (stopwatch.ElapsedMilliseconds + sleepDuration >= maximumDuration)
            {
                return;
            }
            Thread.Sleep(sleepDuration);

            // update individual child cares for the rest of the time
            IList<long> times = new List<long>();
            long expectedEnd = stopwatch.ElapsedMilliseconds;
            while (expectedEnd < maximumDuration)
            {
                long before = stopwatch.ElapsedMilliseconds;

                // update a child care and sleep (if possible)
                Update(false);
                if (stopwatch.ElapsedMilliseconds + sleepDuration >= maximumDuration)
                {
                    return;
                }
                Thread.Sleep(sleepDuration);

                times.Add(stopwatch.ElapsedMilliseconds - before);
                expectedEnd = (long) times.Average() + stopwatch.ElapsedMilliseconds;
            }
        }

        private static void Update(bool updateCounty)
        {
            try
            {
                Logger.Trace("OdjfsDataChecker is now starting.");

                IKernel kernel = new StandardKernel();
                kernel.Bind(c => c
                    .FromAssemblyContaining(typeof (IChildCareStubListScraper))
                    .SelectAllClasses()
                    .BindAllInterfaces());

                var parameter = new ConstructorArgument("odjfsClient", new DownloadingOdjfsClient(@"Logs\HTML"));
                var dataChecker = new Odjfs(kernel.Get<IChildCareStubListScraper>(parameter), kernel.Get<IChildCareScraper>(parameter));

                using (var ctx = new OdjfsEntities())
                {
                    if (updateCounty)
                    {
                        dataChecker.UpdateNextCounty(ctx).Wait();
                    }
                    else
                    {
                        dataChecker.UpdateNextChildCare(ctx).Wait();
                    }
                }

                Logger.Trace("SortaDataChecker has completed.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("An exception has forced the SortaDataChecker to terminate.", e);
            }
        }
    }
}