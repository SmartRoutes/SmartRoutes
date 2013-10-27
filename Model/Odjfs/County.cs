using System;

namespace Model.Odjfs
{
    public class County
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? LastScrapedOn { get; set; }
    }
}