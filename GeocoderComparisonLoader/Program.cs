using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Globalization;
using System.IO;
using System.Linq;
using Database.Contexts;
using Model.Odjfs.ChildCares;

namespace GeocoderComparisonLoader
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            // get the database path
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: {0} GEOCODER DATABASE ONLY_EMPTY", AppDomain.CurrentDomain.FriendlyName);
                Console.Error.WriteLine();
                Console.Error.WriteLine("Load geocoded locations from a GeocoderComparison database into the ODJFS database.");
                Console.Error.WriteLine();
                Console.Error.WriteLine("  GEOCODER     the name of the geocoder to load locations from");
                Console.Error.WriteLine("  DATABASE     the path to the GeocoderComparison database (an .sdf file).");
                Console.Error.WriteLine("  ONLY_EMPTY   only fill in geocoded locations on child cares that are not");
                Console.Error.WriteLine("               already geocoded; must be parsable by Convert.ToBoolean");
                return 1;
            }
            string geocoderName = args[0];
            string databasePath = args[1];
            bool onlyEmpty = Convert.ToBoolean(args[2]);
            if (!File.Exists(databasePath))
            {
                Console.Error.WriteLine("The provided path ('{0}') does not exists.", databasePath);
            }

            // connect to the SQL Server CE database
            string connectionString = string.Format("Data Source={0};Max Database Size=4000;Mode=Read Only;Temp Path={1}",
                databasePath,
                Path.GetTempPath());
            using (var connection = new SqlCeConnection(connectionString))
            {
                connection.Open();

                // connect to the ODFJS database
                using (var entities = new OdjfsEntities())
                {
                    int modifiedCount = 0;
                    foreach (ChildCare childCare in entities.ChildCares.Where(c => c.Address != null && (!onlyEmpty || !c.LastGeocodedOn.HasValue)))
                    {
                        if (TryLoadLocation(connection, geocoderName, childCare))
                        {
                            modifiedCount++;
                        }
                        else
                        {
                            Console.WriteLine("Failed to find a {0} geocoded location for child care {1}.", geocoderName, childCare.ExternalUrlId);
                        }

                        if (modifiedCount%500 == 1)
                        {
                            Console.Write("Saving changes so far...");
                            entities.SaveChanges();
                            Console.WriteLine(" done.");
                        }
                    }

                    Console.Write("Saving the last changes...");
                    entities.SaveChanges();
                    Console.WriteLine(" done.");
                }
            }

            return 0;
        }

        private static bool TryLoadLocation(SqlCeConnection connection, string geocoderName, ChildCare childCare)
        {
            string fullAddress = string.Join(", ", new[]
            {
                childCare.Address,
                childCare.City,
                childCare.State,
                childCare.ZipCode.ToString(CultureInfo.InvariantCulture)
            });
            string zipAddress = string.Join(", ", new[]
            {
                childCare.Address,
                childCare.ZipCode.ToString(CultureInfo.InvariantCulture)
            });

            // try the two address formats in succession, prefering the full address
            GeocodedData geocodedData = GetGeocodedData(connection, geocoderName, fullAddress) ?? GetGeocodedData(connection, geocoderName, zipAddress);
            if (geocodedData == null)
            {
                return false;
            }

            childCare.Latitude = geocodedData.Latitude;
            childCare.Longitude = geocodedData.Longitude;
            childCare.LastGeocodedOn = geocodedData.LastGeocodedOn;
            return true;
        }

        private static GeocodedData GetGeocodedData(SqlCeConnection connection, string geocoderName, string query)
        {
            using (SqlCeCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                SELECT
                    MAX(Location.Latitude),
                    MAX(Location.Longitude),
                    MAX(Location.CreatedOn)
                FROM Query
                INNER JOIN Request ON Query.QueryId = Request.QueryId
                INNER JOIN Geocoder ON Request.GeocoderId = Geocoder.GeocoderId 
                INNER JOIN Location ON Request.RequestId = Location.RequestId
                WHERE
                    Query.Text = @Query AND
                    Geocoder.Name = @GeocoderName
                GROUP BY Location.ResponseId
                HAVING COUNT(*) = 1";
                command.Parameters.Add("@Query", SqlDbType.NVarChar).Value = query;
                command.Parameters.Add("@GeocoderName", SqlDbType.NVarChar).Value = geocoderName;

                using (SqlCeDataReader reader = command.ExecuteReader())
                {
                    bool hasResult = reader.Read();
                    if (!hasResult)
                    {
                        return null;
                    }

                    return new GeocodedData(reader.GetDouble(0), reader.GetDouble(1), reader.GetDateTime(2));
                }
            }
        }
    }
}