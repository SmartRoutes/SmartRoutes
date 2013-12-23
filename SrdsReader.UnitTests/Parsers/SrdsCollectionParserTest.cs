using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.SrdsReader.Parsers;
using SmartRoutes.SrdsReader.Support;

namespace SrdsReader.UnitTests.Parsers
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
            Assert.AreEqual(1, srdsCollection.AttributeKeys.Count());
            Assert.AreEqual("Description", srdsCollection.AttributeKeys.First().Name);

            Assert.IsNotNull(srdsCollection.AttributeValues);
            Assert.AreEqual(1, srdsCollection.AttributeValues.Count());
            Assert.AreEqual("Description", srdsCollection.AttributeValues.First().AttributeKey.Name);
            Assert.AreEqual("Bar", srdsCollection.AttributeValues.First().Value);

            Assert.IsNotNull(srdsCollection.Destinations);
            Assert.AreEqual(1, srdsCollection.Destinations.Count());
            Assert.AreEqual("Foo", srdsCollection.Destinations.First().Name);
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
            Assert.AreEqual(0, srdsCollection.AttributeKeys.Count());

            Assert.IsNotNull(srdsCollection.AttributeValues);
            Assert.AreEqual(0, srdsCollection.AttributeValues.Count());

            Assert.IsNotNull(srdsCollection.Destinations);
            Assert.AreEqual(1, srdsCollection.Destinations.Count());
            Assert.AreEqual("Foo", srdsCollection.Destinations.First().Name);
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
            Assert.AreEqual(0, srdsCollection.AttributeKeys.Count());

            Assert.IsNotNull(srdsCollection.AttributeValues);
            Assert.AreEqual(0, srdsCollection.AttributeValues.Count());

            Assert.IsNotNull(srdsCollection.Destinations);
            Assert.AreEqual(0, srdsCollection.Destinations.Count());
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