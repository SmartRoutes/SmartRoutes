using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    [TestClass]
    public class BaseChildCareParserTest
    {
        [TestMethod]
        public void HappyPath()
        {
        }

        private void VerifyAreEqual(ChildCare expected, ChildCare actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.County.Name, actual.County.Name);
            Assert.AreEqual(expected.ExternalUrlId, actual.ExternalUrlId);
            Assert.AreEqual(expected.ExternalId, actual.ExternalId);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.City, actual.City);
            Assert.AreEqual(expected.State, actual.State);
            Assert.AreEqual(expected.ZipCode, actual.ZipCode);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
        }
    }
}