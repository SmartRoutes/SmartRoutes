using Microsoft.VisualStudio.TestTools.UnitTesting;
using OdjfsScraper.Parsers;
using OdjfsScraper.Test.Parsers.Support;
using SmartRoutes.Model.Odjfs.ChildCares;

namespace OdjfsScraper.Test.Parsers
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