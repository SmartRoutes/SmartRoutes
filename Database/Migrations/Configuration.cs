using System.Data.Entity.Migrations;

namespace SmartRoutes.Database.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Entities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Entities context)
        {
        }
    }
}