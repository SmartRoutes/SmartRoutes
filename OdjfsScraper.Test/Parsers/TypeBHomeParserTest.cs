using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Test.Parsers.Support;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    [TestClass]
    public class TypeAHomeParserTest : BaseChildCareParserTest<TypeAHome, TypeAHomeTemplate, TypeAHomeParser>
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
    }
}