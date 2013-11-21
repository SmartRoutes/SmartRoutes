using System.Data.Entity.Migrations;
using SmartRoutes.Database.Contexts;

namespace SmartRoutes.Database.Migrations.Sorta
{
    internal sealed class SortaMigrationsConfiguration : DbMigrationsConfiguration<SortaEntities>
    {
        public SortaMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SortaEntities context)
        {
        }
    }
}