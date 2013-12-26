using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Reader.Parsers.Srds;

namespace SmartRoutes.Reader.UnitTests.Parsers.Srds
{
    [TestClass]
    public class StringParserTest
    {
        [TestMethod]
        public void Parse_String()
        {
            // ARRANGE
            var parser = new StringParser();
            const string expected = "Foo";

            // ACT
            object output = parser.Parse("System.String", expected);

            // ASSERT
            var actual = output as string;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Parse_NoParser()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT
            try
            {
                parser.Parse("Voynich.Manuscript", "foo");

                // ASSERT
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void SupportsParsing_True_String()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT, ASSERT
            Assert.IsTrue(parser.SupportsParsing("System.String"));
        }

        [TestMethod]
        public void SupportsParsing_False()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT, ASSERT
            Assert.IsFalse(parser.SupportsParsing("Voynich.Manuscript"));
        }
    }
}