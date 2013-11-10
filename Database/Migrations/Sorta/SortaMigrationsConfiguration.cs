using System.Data.Entity.Migrations;
using Database.Contexts;

namespace Database.Migrations.Sorta
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