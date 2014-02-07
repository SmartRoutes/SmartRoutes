using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using OdjfsScraper.Database;
using SmartRoutes.Demo.OdjfsDatabase.Model;

namespace SmartRoutes.Demo.OdjfsDatabase
{
    public class OdjfsDatabase : IDisposable
    {
        private readonly string _connectionString;
        private Entities _ctx;

        public OdjfsDatabase(string connectionString)
        {
            _connectionString = connectionString;
            _ctx = null;
        }

        public void Dispose()
        {
            if (_ctx != null)
            {
                _ctx.Dispose();
            }
        }

        public async Task<IEnumerable<ChildCare>> GetChildCares()
        {
            await Open();

            IEnumerable<OdjfsScraper.Model.ChildCares.ChildCare> inputChildCares = await _ctx
                .ChildCares
                .Where(c => c.Latitude.HasValue && c.Longitude.HasValue)
                .ToArrayAsync();

            IList<ChildCare> outputChildCares = new List<ChildCare>();

            // map each child care to its appropriote wrapper type
            foreach (OdjfsScraper.Model.ChildCares.ChildCare childCare in inputChildCares)
            {
                var typeAHome = childCare as OdjfsScraper.Model.ChildCares.TypeAHome;
                if (typeAHome != null)
                {
                    outputChildCares.Add(new TypeAHome(typeAHome));
                }

                var typeBHome = childCare as OdjfsScraper.Model.ChildCares.TypeBHome;
                if (typeBHome != null)
                {
                    outputChildCares.Add(new TypeBHome(typeBHome));
                }

                var licensedCenter = childCare as OdjfsScraper.Model.ChildCares.LicensedCenter;
                if (licensedCenter != null)
                {
                    outputChildCares.Add(new LicensedCenter(licensedCenter));
                }

                var dayCamp = childCare as OdjfsScraper.Model.ChildCares.DayCamp;
                if (dayCamp != null)
                {
                    outputChildCares.Add(new DayCamp(dayCamp));
                }
            }

            return outputChildCares;
        }

        public void Close()
        {
            Dispose();
        }

        public async Task Open()
        {
            if (_ctx == null)
            {
                _ctx = new Entities();
                _ctx.Database.Connection.ConnectionString = _connectionString;
                await _ctx.Database.Connection.OpenAsync();
            }
        }
    }
}