using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Srds;
using SmartRoutes.Reader.Parsers.Srds;

namespace SmartRoutes.Reader.UnitTests.Parsers.Srds
{
    [TestClass]
    public class SrdsCollectionParserTest
    {
        [TestMethod]
        public void HappyPath()
        {
            // ARRANGE
            byte[] zipFileBytes = GetZipFileBytes(new[] {"Name,TypeName", "Description,System.String"}, new[] {"Name,Latitude,Longitude,Description", "Foo,0.0,0.0,Bar"});
            SrdsCollectionParser parser = GetParser();

            // ACT
            SrdsCollection srdsCollection = parser.Parse(zipFileBytes);

            // ASSERT
            Assert.IsNotNull(srdsCollection.AttributeKeys);
            Assert.AreEqual(1, srdsCollection.AttributeKeys.Length);
            Assert.AreEqual(1, srdsCollection.AttributeKeys[0].Id);
            Assert.AreEqual("Description", srdsCollection.AttributeKeys[0].Name);

            Assert.IsNotNull(srdsCollection.AttributeValues);
            Assert.AreEqual(1, srdsCollection.AttributeValues.Length);
            Assert.AreEqual(1, srdsCollection.AttributeValues[0].Id);
            Assert.AreEqual("Bar", srdsCollection.AttributeValues[0].Value);
            Assert.AreEqual("Description", srdsCollection.AttributeValues[0].AttributeKey.Name);

            Assert.IsNotNull(srdsCollection.Destinations);
            Assert.AreEqual(1, srdsCollection.Destinations.Length);
            Assert.AreEqual(1, srdsCollection.Destinations[0].Id);
            Assert.AreEqual("Foo", srdsCollection.Destinations[0].Name);
        }

        [TestMethod]
        public void TwoDestinationsTwoAttributes()
        {
            // ARRANGE
            byte[] zipFileBytes = GetZipFileBytes(
                new[]
                {
                    "Name,TypeName",
                    "Description,System.String",
                    "Title,System.String"
                },
                new[]
                {
                    "Name,Latitude,Longitude,Description,Title",
                    "Foo,0.0,0.0,Bar,Baz",
                    "Boo,0.0,0.0,Far,Faz"
                });
            SrdsCollectionParser parser = GetParser();

            // ACT
            SrdsCollection srdsCollection = parser.Parse(zipFileBytes);

            // ASSERT
            Assert.IsNotNull(srdsCollection.AttributeKeys);
            Assert.AreEqual(2, srdsCollection.AttributeKeys.Length);
            Assert.AreEqual(1, srdsCollection.AttributeKeys[0].Id);
            Assert.AreEqual(2, srdsCollection.AttributeKeys[1].Id);
            Assert.AreEqual("Description", srdsCollection.AttributeKeys[0].Name);
            Assert.AreEqual("Title", srdsCollection.AttributeKeys[1].Name);

            Assert.IsNotNull(srdsCollection.AttributeValues);
            Assert.AreEqual(4, srdsCollection.AttributeValues.Length);
            Assert.AreEqual(1, srdsCollection.AttributeValues[0].Id);
            Assert.AreEqual(2, srdsCollection.AttributeValues[1].Id);
            Assert.AreEqual(3, srdsCollection.AttributeValues[2].Id);
            Assert.AreEqual(4, srdsCollection.AttributeValues[3].Id);
            Assert.AreEqual("Bar", srdsCollection.AttributeValues[0].Value);
            Assert.AreEqual("Baz", srdsCollection.AttributeValues[1].Value);
            Assert.AreEqual("Far", srdsCollection.AttributeValues[2].Value);
            Assert.AreEqual("Faz", srdsCollection.AttributeValues[3].Value);
            Assert.AreEqual("Description", srdsCollection.AttributeValues[0].AttributeKey.Name);
            Assert.AreEqual("Title", srdsCollection.AttributeValues[1].AttributeKey.Name);
            Assert.AreEqual("Description", srdsCollection.AttributeValues[2].AttributeKey.Name);
            Assert.AreEqual("Title", srdsCollection.AttributeValues[3].AttributeKey.Name);

            Assert.IsNotNull(srdsCollection.Destinations);
            Assert.AreEqual(2, srdsCollection.Destinations.Length);
            Assert.AreEqual(1, srdsCollection.Destinations[0].Id);
            Assert.AreEqual(2, srdsCollection.Destinations[1].Id);
            Assert.AreEqual("Foo", srdsCollection.Destinations[0].Name);
            Assert.AreEqual("Boo", srdsCollection.Destinations[1].Name);
        }

        [TestMethod]
        public void NoAttributeKeys()
        {
            // ARRANGE
            byte[] zipFileBytes = GetZipFileBytes(new[] {"Name,TypeName"}, new[] {"Name,Latitude,Longitude", "Foo,0.0,0.0"});
            SrdsCollectionParser parser = GetParser();

            // ACT
            SrdsCollection srdsCollection = parser.Parse(zipFileBytes);

            // ASSERT
            Assert.IsNotNull(srdsCollection.AttributeKeys);
            Assert.AreEqual(0, srdsCollection.AttributeKeys.Length);

            Assert.IsNotNull(srdsCollection.AttributeValues);
            Assert.AreEqual(0, srdsCollection.AttributeValues.Length);

            Assert.IsNotNull(srdsCollection.Destinations);
            Assert.AreEqual(1, srdsCollection.Destinations.Length);
            Assert.AreEqual("Foo", srdsCollection.Destinations[0].Name);
        }

        [TestMethod]
        public void OnlyHeaders()
        {
            // ARRANGE
            byte[] zipFileBytes = GetZipFileBytes(new[] {"Name,TypeName"}, new[] {"Name,Latitude,Longitude"});
            SrdsCollectionParser parser = GetParser();

            // ACT
            SrdsCollection srdsCollection = parser.Parse(zipFileBytes);

            // ASSERT
            Assert.IsNotNull(srdsCollection.AttributeKeys);
            Assert.AreEqual(0, srdsCollection.AttributeKeys.Length);

            Assert.IsNotNull(srdsCollection.AttributeValues);
            Assert.AreEqual(0, srdsCollection.AttributeValues.Length);

            Assert.IsNotNull(srdsCollection.Destinations);
            Assert.AreEqual(0, srdsCollection.Destinations.Length);
        }


        private static SrdsCollectionParser GetParser()
        {
            return new SrdsCollectionParser(new AttributeKeyCsvStreamParser(), new DestinationCsvStreamParser(new StringParser()));
        }

        private static byte[] GetZipFileBytes(IEnumerable<string> attibuteKeyLines, IEnumerable<string> destinationLines)
        {
            using (var zip = new ZipFile())
            {
                zip.AddEntry("AttributeKeys.csv", string.Join(Environment.NewLine, attibuteKeyLines));
                zip.AddEntry("Destinations.csv", string.Join(Environment.NewLine, destinationLines));

                var output = new MemoryStream();
                zip.Save(output);

                return output.ToArray();
            }
        }
    }
}