using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Test.Parsers.Support;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    [TestClass]
    public class BaseChildCareParserTest
    {
        [TestMethod]
        public void HappyPath()
        {
            // ARRANGE
            var template = new TypeBHomeTemplate();
            byte[] document = null;
            var parser = new BaseChildCareParser<ChildCare>();

            // ACT
            ChildCare childCare = parser.Parse(new DayCamp(), document);
        }
    }
}