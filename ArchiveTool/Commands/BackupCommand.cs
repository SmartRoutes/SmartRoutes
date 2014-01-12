using System;
using System.Data.Entity;
using System.Data.SqlClient;
using ManyConsole;
using NLog;
using SmartRoutes.Database;

namespace SmartRoutes.ArchiveTool.Commands
{
    public class BackupCommand : DatabaseCommand
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BackupCommand()
        {
            IsCommand("backup", "backup the database to a .bak file");
            HasRequiredOption("path=", "the file path of where the backup will be written", v => Path = v);
        }

        public string Path { get; set; }

        public override int Run(string[] remainingArguments)
        {
            string fullPath = System.IO.Path.GetFullPath(Path);
            Logger.Trace("The export will be created at the following location:{0}  {1}", Environment.NewLine, fullPath);

            using (var ctx = new Entities())
            {
                ctx.Database.ExecuteSqlCommand(
                    TransactionalBehavior.DoNotEnsureTransaction,
                    "BACKUP DATABASE @Database TO DISK = @Path WITH COPY_ONLY, INIT",
                    new SqlParameter("@Database", ctx.Database.Connection.Database),
                    new SqlParameter("@Path", fullPath));
            }

            return 0;
        }
    }
}