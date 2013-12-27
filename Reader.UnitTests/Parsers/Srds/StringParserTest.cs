using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Reader.Parsers.Srds;

namespace SmartRoutes.Reader.UnitTests.Parsers.Srds
{
    [TestClass]
    public class StringParserTest
    {
        private static readonly ISet<string> SupportedClasses = new HashSet<string>
        {
            "System.String"
        };

        private static readonly ISet<string> SupporedStructs = new HashSet<string>
        {
            "System.Boolean", "System.DateTime", "System.Int32", "System.Double", "System.Single"
        };

        [TestMethod]
        public void Parse_Valid()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT
            object output = parser.Parse("System.String", "Foo");

            // ASSERT
            var actual = output as string;
            Assert.IsNotNull(actual);
            Assert.AreEqual("Foo", actual);
        }

        [TestMethod]
        public void Parse_NoParser()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT
            try
            {
                parser.Parse("Voynich.Manuscript", "Foo");

                // ASSERT
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void Parse_Invalid()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT
            try
            {
                parser.Parse("System.Int32", "Foo");

                // ASSERT
                Assert.Fail();
            }
            catch (FormatException)
            {
            }
        }

        [TestMethod]
        public void Parse_Nullable_WithValue()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT
            object output = parser.Parse("System.Int32?", "23");

            // ASSERT
            var actual = output as int?;
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.HasValue);
            Assert.AreEqual(23, actual.Value);
        }

        [TestMethod]
        public void Parse_Nullable_WithoutValue_All()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT, ASSERT
            foreach (string type in SupporedStructs)
            {
                object output = parser.Parse(type + "?", string.Empty);
                Assert.IsNull(output);
            }
        }

        [TestMethod]
        public void SupportsParsing_True_All()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT, ASSERT
            foreach (string type in SupporedStructs.Concat(SupportedClasses))
            {
                Assert.IsTrue(parser.SupportsParsing(type), string.Format("Unsupported type: {0}", type));
            }
        }

        [TestMethod]
        public void SupportsParsing_True_Nullable()
        {
            // ARRANGE
            var parser = new StringParser();

            // ACT, ASSERT
            foreach (string type in SupporedStructs)
            {
                Assert.IsTrue(parser.SupportsParsing(type + "?"), string.Format("Unsupported type: {0}?", type));
            }
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