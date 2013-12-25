using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Database.Data;
using SmartRoutes.Database.UnitTests.Data.TestSupport;

namespace SmartRoutes.Database.UnitTests.Data
{
    [TestClass]
    public class CsvWriter
    {
        [TestMethod]
        public void TwoRecords()
        {
            // ARRANGE
            var stream = new MemoryStream();
            using (var csvWriter = new CsvWriter<Country>(stream))
            {
                // ACT
                csvWriter.Write(new Country {Id = 1, Name = "Netherlands"});
                csvWriter.Write(new Country {Id = 2, Name = "Germany"});
            }

            // ASSERT
            string[] lines = GetLines(stream);
            Assert.IsNotNull(lines);
            Assert.AreEqual(3, lines.Length);
            Assert.AreEqual("Id,Name", lines[0]);
            Assert.AreEqual("1,Netherlands", lines[1]);
            Assert.AreEqual("2,Germany", lines[2]);
        }

        [TestMethod]
        public void OneRecord()
        {
            // ARRANGE
            var stream = new MemoryStream();
            using (var csvWriter = new CsvWriter<Country>(stream))
            {
                // ACT
                csvWriter.Write(new Country {Id = 1, Name = "Netherlands"});
            }

            // ASSERT
            string[] lines = GetLines(stream);
            Assert.IsNotNull(lines);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("Id,Name", lines[0]);
            Assert.AreEqual("1,Netherlands", lines[1]);
        }

        [TestMethod]
        public void InnerQuotes()
        {
            // ARRANGE
            var stream = new MemoryStream();
            using (var csvWriter = new CsvWriter<Country>(stream))
            {
                // ACT
                csvWriter.Write(new Country {Id = 1, Name = "Foo \"Bar\" Baz"});
            }

            // ASSERT
            string[] lines = GetLines(stream);
            Assert.IsNotNull(lines);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("Id,Name", lines[0]);
            Assert.AreEqual("1,\"Foo \"\"Bar\"\" Baz\"", lines[1]);
        }

        [TestMethod]
        public void OuterQuotes()
        {
            // ARRANGE
            var stream = new MemoryStream();
            using (var csvWriter = new CsvWriter<Country>(stream))
            {
                // ACT
                csvWriter.Write(new Country {Id = 1, Name = "\"Foo\""});
            }

            // ASSERT
            string[] lines = GetLines(stream);
            Assert.IsNotNull(lines);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("Id,Name", lines[0]);
            Assert.AreEqual("1,\"\"\"Foo\"\"\"", lines[1]);
        }

        [TestMethod]
        public void Commas()
        {
            // ARRANGE
            var stream = new MemoryStream();
            using (var csvWriter = new CsvWriter<Country>(stream))
            {
                // ACT
                csvWriter.Write(new Country {Id = 1, Name = "Foo, Bar"});
            }

            // ASSERT
            string[] lines = GetLines(stream);
            Assert.IsNotNull(lines);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("Id,Name", lines[0]);
            Assert.AreEqual("1,\"Foo, Bar\"", lines[1]);
        }

        [TestMethod]
        public void CommasOuterQuotes()
        {
            // ARRANGE
            var stream = new MemoryStream();
            using (var csvWriter = new CsvWriter<Country>(stream))
            {
                // ACT
                csvWriter.Write(new Country {Id = 1, Name = "\"Foo, Bar\""});
            }

            // ASSERT
            string[] lines = GetLines(stream);
            Assert.IsNotNull(lines);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("Id,Name", lines[0]);
            Assert.AreEqual("1,\"\"\"Foo, Bar\"\"\"", lines[1]);
        }

        [TestMethod]
        public void CommasInnerQuotes()
        {
            // ARRANGE
            var stream = new MemoryStream();
            using (var csvWriter = new CsvWriter<Country>(stream))
            {
                // ACT
                csvWriter.Write(new Country {Id = 1, Name = "Foo \"Bar, Bar\" Baz"});
            }

            // ASSERT
            string[] lines = GetLines(stream);
            Assert.IsNotNull(lines);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("Id,Name", lines[0]);
            Assert.AreEqual("1,\"Foo \"\"Bar, Bar\"\" Baz\"", lines[1]);
        }

        private string[] GetLines(MemoryStream stream)
        {
            var reader = new StreamReader(new MemoryStream(stream.ToArray()));
            var lines = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return lines.ToArray();
        }
    }
}