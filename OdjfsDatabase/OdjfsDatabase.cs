using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using OdjfsScraper.Database;
using SmartRoutes.Demo.OdjfsDatabase.Model;
using DayCamp = OdjfsScraper.Model.ChildCares.DayCamp;
using LicensedCenter = OdjfsScraper.Model.ChildCares.LicensedCenter;
using TypeAHome = OdjfsScraper.Model.ChildCares.TypeAHome;
using TypeBHome = OdjfsScraper.Model.ChildCares.TypeBHome;

namespace SmartRoutes.Demo.OdjfsDatabase
{
    public class OdjfsDatabase : IDisposable
    {
        private readonly string _nameOrConnectionString;
        private Entities _ctx;

        public OdjfsDatabase(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
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
            Open();

            IEnumerable<OdjfsScraper.Model.ChildCares.ChildCare> inputChildCares = await _ctx
                .ChildCares
                .AsNoTracking()
                .Where(c => c.Latitude.HasValue && c.Longitude.HasValue)
                .ToArrayAsync();

            IList<ChildCare> outputChildCares = new List<ChildCare>();

            // map each child care to its appropriote wrapper type
            foreach (OdjfsScraper.Model.ChildCares.ChildCare childCare in inputChildCares)
            {
                var typeAHome = childCare as TypeAHome;
                if (typeAHome != null)
                {
                    outputChildCares.Add(new Model.TypeAHome(typeAHome));
                }

                var typeBHome = childCare as TypeBHome;
                if (typeBHome != null)
                {
                    outputChildCares.Add(new Model.TypeBHome(typeBHome));
                }

                var licensedCenter = childCare as LicensedCenter;
                if (licensedCenter != null)
                {
                    outputChildCares.Add(new Model.LicensedCenter(licensedCenter));
                }

                var dayCamp = childCare as DayCamp;
                if (dayCamp != null)
                {
                    outputChildCares.Add(new Model.DayCamp(dayCamp));
                }
            }

            return outputChildCares;
        }

        public void Close()
        {
            Dispose();
        }

        public void Open()
        {
            if (_ctx == null)
            {
                _ctx = new Entities(_nameOrConnectionString);
            }
        }
    }
}