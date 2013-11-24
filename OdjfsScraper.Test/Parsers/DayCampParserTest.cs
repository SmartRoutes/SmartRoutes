using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Test.Parsers.Support;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    [TestClass]
    public class DayCampParserTest : BaseChildCareParserTest<DayCamp, DayCampTemplate, DayCampParser>
    {
        [TestMethod]
        public override void HappyPath()
        {
            base.HappyPath();
        }

        [TestMethod]
        public override void MissingDetail()
        {
            base.MissingDetail();
        }

        protected override void VerifyAreEqual(DayCamp expected, DayCamp actual)
        {
            base.VerifyAreEqual(expected, actual);

            Assert.AreEqual(expected.RegistrationStatus, actual.RegistrationStatus);
            Assert.AreEqual(expected.Owner, actual.Owner);
            Assert.AreEqual(expected.RegistrationBeginDate, actual.RegistrationBeginDate);
            Assert.AreEqual(expected.RegistrationEndDate, actual.RegistrationEndDate);
        }
    }
}