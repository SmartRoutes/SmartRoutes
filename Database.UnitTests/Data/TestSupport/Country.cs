using System.Collections.Generic;

namespace SmartRoutes.Database.UnitTests.Data.TestSupport
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<object> ChildObjects { get; set; }
    }
}