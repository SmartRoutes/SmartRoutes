using System;

namespace SmartRoutes.Model
{
    public abstract class Archive
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public DateTime LoadedOn { get; set; }
        public string ArchiveType { get; set; }
    }
}