using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Srds;
using SmartRoutes.Scraper.Test.Support;
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

        [TestMethod]
        public void NoRows()
        {
            // ARRANGE
            var parser = new AttributeKeyCsvStreamParser();
            var lines = new[]
            {
                "Name,TypeName"
            };
            var expected = new AttributeKey[0];

            // ACT
            AttributeKey[] actual = Parse(parser, lines);

            // ASSERT
            AssertAreEqual(expected, actual);
        }

        [TestMethod]
        public void MissingNameColumn()
        {
            // ARRANGE
            var parser = new AttributeKeyCsvStreamParser();
            var lines = new[]
            {
                "TypeName",
                "System.Boolean"
            };

            // ACT
            try
            {
                Parse(parser, lines);

                // ASSERT
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void MissingTypeNameColumn()
        {
            // ARRANGE
            var parser = new AttributeKeyCsvStreamParser();
            var lines = new[]
            {
                "Name",
                "IsCool"
            };

            // ACT
            try
            {
                Parse(parser, lines);

                // ASSERT
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }
        }

        protected override void AssertAreEqual(AttributeKey expected, AttributeKey actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.TypeName, actual.TypeName);
        }
    }
}