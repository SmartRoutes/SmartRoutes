using System;

namespace Model.Sorta
{
    public class Archive
    {
        public int Id { get; set; }
        public string LastModified { get; set; }
        public string ETag { get; set; }
        public string Hash { get; set; }
        public DateTime DownloadedOn { get; set; }
    }
}