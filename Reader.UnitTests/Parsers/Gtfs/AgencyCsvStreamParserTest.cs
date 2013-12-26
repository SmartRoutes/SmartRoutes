using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Gtfs;
using SmartRoutes.Reader.Parsers.Gtfs;
using SmartRoutes.Reader.UnitTests.Parsers.TestSupport;

namespace SmartRoutes.Reader.UnitTests.Parsers.Gtfs
{
    [TestClass]
    public class AgencyCsvStreamParserTest : AbstractCsvStreamParserTest<Agency>
    {
        [TestMethod]
        public void HappyPath()
        {
            // ARRANGE
            var parser = new AgencyCsvStreamParser();
            var lines = new[]
            {
                "agency_id,agency_name,agency_url,agency_timezone,agency_lang,agency_phone,agency_fare_url,",
                "ID,NAME,AGENCY_URL,TIMEZONE,LANG,PHONE,FARE_URL,"
            };
            var expected = new[]
            {
                new Agency
                {
                    Id = "ID",
                    Name = "NAME",
                    Url = "AGENCY_URL",
                    Timezone = "TIMEZONE",
                    Language = "LANG",
                    Phone = "PHONE",
                    FareUrl = "FARE_URL"
                }
            };

            // ACT
            Agency[] actual = Parse(parser, lines);

            // ASSERT
            AssertAreEqual(expected, actual);
        }

        protected override void AssertAreEqual(Agency expected, Agency actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Url, actual.Url);
            Assert.AreEqual(expected.Timezone, actual.Timezone);
            Assert.AreEqual(expected.Language, actual.Language);
            Assert.AreEqual(expected.Phone, actual.Phone);
            Assert.AreEqual(expected.FareUrl, actual.FareUrl);
            Assert.IsNotNull(actual.Routes);
            Assert.AreEqual(0, actual.Routes.Count);
        }
    }
}