using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.UnitTests.Support;
using SmartRoutes.SrdsReader.Parsers;

namespace SrdsReader.UnitTests.Parsers
{
    [TestClass]
    public class AttributeKeyCsvStreamParserTest : AbstractCsvStreamParserTest<AttributeKey>
    {
        [TestMethod]
        public void HappyPath()
        {
            // ARRANGE
            var parser = new AttributeKeyCsvStreamParser();
            var lines = new[]
            {
                "Name,TypeName",
                "IsCool,System.Boolean"
            };
            var expected = new[]
            {
                new AttributeKey
                {
                    Name = "IsCool",
                    TypeName = "System.Boolean"
                }
            };

            // ACT
            AttributeKey[] actual = Parse(parser, lines);

            // ASSERT
            AssertAreEqual(expected, actual);
        }

        protected override void AssertAreEqual(AttributeKey expected, AttributeKey actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.TypeName, actual.TypeName);
        }
    }
}