using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.UnitTests.Parsers.TestSupport;
using SmartRoutes.Reader.Parsers.Srds;

namespace SmartRoutes.Reader.UnitTests.Parsers.Srds
{
    [TestClass]
    public class DestinationCsvStreamParserTest : AbstractCsvStreamParserTest<Destination>
    {
        [TestMethod]
        public void HappyPath()
        {
            // ARRANGE
            var parser = new DestinationCsvStreamParser(new StringParser());
            var lines = new[]
            {
                "Name,Latitude,Longitude",
                "Foo,0.0,0.0"
            };
            var expected = new[]
            {
                new Destination
                {
                    Name = "Foo",
                    Latitude = 0.0,
                    Longitude = 0.0
                }
            };

            // ACT
            Destination[] actual = Parse(parser, lines);

            // ASSERT
            AssertAreEqual(expected, actual);
        }

        [TestMethod]
        public void Attributes_One()
        {
            // ARRANGE
            var parser = new DestinationCsvStreamParser(new StringParser());
            parser.AttachAttributeKeys(new[]
            {
                new AttributeKey {Name = "Description", TypeName = "System.String"}
            });
            var lines = new[]
            {
                "Name,Latitude,Longitude,Description",
                "Foo,0.0,0.0,Bar"
            };
            var expected = new[]
            {
                new Destination
                {
                    Name = "Foo",
                    Latitude = 0.0,
                    Longitude = 0.0,
                    AttributeValues = new[]
                    {
                        new AttributeValue
                        {
                            AttributeKey = new AttributeKey {Name = "Description"},
                            Value = "Bar",
                            ValueString = "Bar"
                        }
                    }
                }
            };

            // ACT
            Destination[] actual = Parse(parser, lines);

            // ASSERT
            AssertAreEqual(expected, actual);
        }

        [TestMethod]
        public void Attributes_Two()
        {
            // ARRANGE
            var parser = new DestinationCsvStreamParser(new StringParser());
            parser.AttachAttributeKeys(new[]
            {
                new AttributeKey {Name = "Description", TypeName = "System.String"},
                new AttributeKey {Name = "IsCool", TypeName = "System.Boolean"}
            });
            var lines = new[]
            {
                "Name,Latitude,Longitude,Description,IsCool",
                "Foo,0.0,0.0,Bar,True"
            };
            var expected = new[]
            {
                new Destination
                {
                    Name = "Foo",
                    Latitude = 0.0,
                    Longitude = 0.0,
                    AttributeValues = new[]
                    {
                        new AttributeValue
                        {
                            AttributeKey = new AttributeKey {Name = "Description"},
                            Value = "Bar",
                            ValueString = "Bar"
                        },
                        new AttributeValue
                        {
                            AttributeKey = new AttributeKey {Name = "IsCool"},
                            Value = true,
                            ValueString = "True"
                        }
                    }
                }
            };

            // ACT
            Destination[] actual = Parse(parser, lines);

            // ASSERT
            AssertAreEqual(expected, actual);
        }

        [TestMethod]
        public void Attribute_Unsupported()
        {
            // ARRANGE
            var parser = new DestinationCsvStreamParser(new StringParser());

            try
            {
                // ACT
                parser.AttachAttributeKeys(new[]
                {
                    new AttributeKey {Name = "IsCool", TypeName = "Foo.Bar"}
                });

                // ASSERT
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("An AttributeKey TypeName does not have a supported string parser."));
            }
        }

        [TestMethod]
        public void Attribute_Reserved()
        {
            // ARRANGE
            var parser = new DestinationCsvStreamParser(new StringParser());

            try
            {
                // ACT
                parser.AttachAttributeKeys(new[]
                {
                    new AttributeKey {Name = "Id", TypeName = "System.Int32"}
                });

                // ASSERT
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("A reserved AttributeKey name was used."));
            }
        }

        [TestMethod]
        public void Attribute_Duplicate()
        {
            // ARRANGE
            var parser = new DestinationCsvStreamParser(new StringParser());

            try
            {
                // ACT
                parser.AttachAttributeKeys(new[]
                {
                    new AttributeKey {Name = "Description", TypeName = "System.Int32",},
                    new AttributeKey {Name = "Description", TypeName = "System.String",}
                });

                // ASSERT
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Duplicate AttributeKey names are not allowed."));
            }
        }

        protected override void AssertAreEqual(Destination expected, Destination actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Latitude, actual.Latitude);
            Assert.AreEqual(expected.Longitude, actual.Longitude);
            Assert.AreEqual(expected.Name, actual.Name);

            foreach (AttributeValue attributeValue in actual.AttributeValues)
            {
                Assert.IsNotNull(attributeValue);
                Assert.IsNotNull(attributeValue.AttributeKey);
            }

            AttributeValue[] expectedAttributeValues = GetSortedAttributeValues(expected);
            AttributeValue[] actualAttributeValues = GetSortedAttributeValues(actual);

            Assert.AreEqual(expectedAttributeValues.Length, actualAttributeValues.Length);
            for (int i = 0; i < expectedAttributeValues.Length; i++)
            {
                AssertAreEqual(expectedAttributeValues[i], actualAttributeValues[i]);
            }
        }

        private void AssertAreEqual(AttributeValue expected, AttributeValue actual)
        {
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.AttributeKey);
            Assert.AreEqual(expected.AttributeKey.Name, actual.AttributeKey.Name);
            Assert.AreEqual(expected.Value, actual.Value);
            Assert.AreEqual(expected.ValueString, actual.ValueString);
        }

        private AttributeValue[] GetSortedAttributeValues(Destination destination)
        {
            return destination
                .AttributeValues
                .OrderBy(a => a.AttributeKey.Name)
                .ToArray();
        }
    }
}