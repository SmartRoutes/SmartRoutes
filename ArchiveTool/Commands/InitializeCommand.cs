using System.Data.Entity.Migrations;
using System.Linq;
using ManyConsole;
using NLog;
using SmartRoutes.Database.Migrations;

namespace SmartRoutes.ArchiveTool.Commands
{
    public class InitializeCommand : ConsoleCommand, ICommand
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public InitializeCommand()
        {
            IsCommand("init", "initialize the database or update it to the latest schema");
        }

        public override int Run(string[] remainingArguments)
        {
            var configuration = new Configuration();

            Logger.Trace("Detecting the current database configuration.");
            var migrator = new DbMigrator(configuration);
            string[] pendingMigrations = migrator.GetPendingMigrations().ToArray();
            string[] databaseMigrations = migrator.GetDatabaseMigrations().ToArray();

            if (pendingMigrations.Length > 0)
            {
                if (databaseMigrations.Length == 0)
                {
                    Logger.Trace("The database has not been created yet. This will happen now.");
                }
                else
                {
                    Logger.Trace("The database schema is out of data and must be migrated. This will happen now.");
                }
                migrator.Update();
                Logger.Trace("The database is now initialized.");
            }
            else
            {
                Logger.Trace("The database is already initialized.");
            }

            return 0;
        }
    }
}