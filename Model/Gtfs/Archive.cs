using System;

namespace SmartRoutes.Model.Gtfs
{
    public class Archive
    {
        public int Id { get; set; }
        public string ETag { get; set; }
        public string Hash { get; set; }
        public DateTime DownloadedOn { get; set; }
    }
}