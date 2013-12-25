using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Database.Data;
using SmartRoutes.Database.UnitTests.Data.TestSupport;

namespace SmartRoutes.Database.UnitTests.Data
{
    [TestClass]
    public class RecordTest
    {
        [TestMethod]
        public void Create_HappyPath()
        {
            // ARRANGE
            var country = new Country
            {
                Id = 1,
                Name = "Netherlands",
                ChildObjects = new object[0] // virtual properties are ignored
            };

            // ACT
            Record record = Record.Create(country);

            // ASSERT
            Assert.IsNotNull(record.Names);
            Assert.AreEqual(2, record.Names.Count());
            Assert.AreEqual("Id", record.Names.ElementAt(0));
            Assert.AreEqual("Name", record.Names.ElementAt(1));

            Assert.IsNotNull(record.Types);
            Assert.AreEqual(2, record.Types.Count());
            Assert.AreEqual(typeof (int), record.Types.ElementAt(0));
            Assert.AreEqual(typeof (string), record.Types.ElementAt(1));

            Assert.IsNotNull(record.Values);
            Assert.AreEqual(2, record.Values.Count());
            Assert.AreEqual(1, record.Values.ElementAt(0));
            Assert.AreEqual("Netherlands", record.Values.ElementAt(1));
        }
    }
}