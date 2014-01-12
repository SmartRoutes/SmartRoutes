using System;
using System.Data.SqlClient;
using NLog;
using SmartRoutes.Database;

namespace SmartRoutes.ArchiveTool.Commands
{
    public class RestoreCommand : DatabaseCommand
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RestoreCommand()
        {
            IsCommand("restore", "restore the database from a .bak file");
            HasRequiredOption("path=", "the file path of where the restore will be read", v => Path = v);
        }

        public string Path { get; set; }

        public override int Run(string[] remainingArguments)
        {
            string fullPath = System.IO.Path.GetFullPath(Path);
            Logger.Trace("The restore will be read from the following location:{0}  {1}", Environment.NewLine, fullPath);

            string connectionString;
            string database;
            using (var ctx = new Entities())
            {
                connectionString = ctx.Database.Connection.ConnectionString;
                database = ctx.Database.Connection.Database;
            }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                conn.ChangeDatabase("master");
                SqlCommand command = conn.CreateCommand();
                command.CommandText = "RESTORE DATABASE @Database FROM DISK = @Path WITH REPLACE";
                command.Parameters.Add(new SqlParameter("@Database", database));
                command.Parameters.Add(new SqlParameter("@Path", fullPath));
                command.ExecuteNonQuery();
            }

            return 0;
        }
    }
}