using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Test.Parsers.Support;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    [TestClass]
    public class TypeBHomeParserTest : BaseChildCareParserTest<TypeBHome, TypeBHomeTemplate, TypeBHomeParser>
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

        protected override void VerifyAreEqual(TypeBHome expected, TypeBHome actual)
        {
            base.VerifyAreEqual(expected, actual);

            Assert.AreEqual(expected.CertificationBeginDate, actual.CertificationBeginDate);
            Assert.AreEqual(expected.CertificationExpirationDate, actual.CertificationExpirationDate);
        }
    }
}