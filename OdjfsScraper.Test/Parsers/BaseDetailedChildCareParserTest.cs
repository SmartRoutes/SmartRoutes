using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Test.Parsers.Support;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    [TestClass]
    public class BaseDetailedChildCareParserTest<TModel, TTemplate, TParser> : BaseChildCareParserTest<TModel, TTemplate, TParser>
        where TModel : DetailedChildCare
        where TTemplate : ChildCareTemplate<TModel>
        where TParser : BaseChildCareParser<TModel>
    {
        protected override void VerifyAreEqual(TModel expected, TModel actual)
        {
            base.VerifyAreEqual(expected, actual);

            Assert.AreEqual(expected.CenterStatus, actual.CenterStatus);
            Assert.AreEqual(expected.Administrators, actual.Administrators);
            Assert.AreEqual(expected.ProviderAgreement, actual.ProviderAgreement);
            Assert.AreEqual(expected.InitialApplicationDate, actual.InitialApplicationDate);
            Assert.AreEqual(expected.LicenseBeginDate, actual.LicenseBeginDate);
            Assert.AreEqual(expected.LicenseExpirationDate, actual.LicenseExpirationDate);
            Assert.AreEqual(expected.SutqRating, actual.SutqRating);

            Assert.AreEqual(expected.Infants, actual.Infants);
            Assert.AreEqual(expected.YoungToddlers, actual.YoungToddlers);
            Assert.AreEqual(expected.OlderToddlers, actual.OlderToddlers);
            Assert.AreEqual(expected.Preschoolers, actual.Preschoolers);
            Assert.AreEqual(expected.Gradeschoolers, actual.Gradeschoolers);
            Assert.AreEqual(expected.ChildCareFoodProgram, actual.ChildCareFoodProgram);

            Assert.AreEqual(expected.Naeyc, actual.Naeyc);
            Assert.AreEqual(expected.Necpa, actual.Necpa);
            Assert.AreEqual(expected.Naccp, actual.Naccp);
            Assert.AreEqual(expected.Nafcc, actual.Nafcc);
            Assert.AreEqual(expected.Coa, actual.Coa);
            Assert.AreEqual(expected.Acsi, actual.Acsi);

            Assert.AreEqual(expected.MondayReported, actual.MondayReported);
            Assert.AreEqual(expected.MondayBegin, actual.MondayBegin);
            Assert.AreEqual(expected.MondayEnd, actual.MondayEnd);

            Assert.AreEqual(expected.TuesdayReported, actual.TuesdayReported);
            Assert.AreEqual(expected.TuesdayBegin, actual.TuesdayBegin);
            Assert.AreEqual(expected.TuesdayEnd, actual.TuesdayEnd);

            Assert.AreEqual(expected.WednesdayReported, actual.WednesdayReported);
            Assert.AreEqual(expected.WednesdayBegin, actual.WednesdayBegin);
            Assert.AreEqual(expected.WednesdayEnd, actual.WednesdayEnd);

            Assert.AreEqual(expected.ThursdayReported, actual.ThursdayReported);
            Assert.AreEqual(expected.ThursdayBegin, actual.ThursdayBegin);
            Assert.AreEqual(expected.ThursdayEnd, actual.ThursdayEnd);

            Assert.AreEqual(expected.FridayReported, actual.FridayReported);
            Assert.AreEqual(expected.FridayBegin, actual.FridayBegin);
            Assert.AreEqual(expected.FridayEnd, actual.FridayEnd);

            Assert.AreEqual(expected.SaturdayReported, actual.SaturdayReported);
            Assert.AreEqual(expected.SaturdayBegin, actual.SaturdayBegin);
            Assert.AreEqual(expected.SaturdayEnd, actual.SaturdayEnd);

            Assert.AreEqual(expected.SundayReported, actual.SundayReported);
            Assert.AreEqual(expected.SundayBegin, actual.SundayBegin);
            Assert.AreEqual(expected.SundayEnd, actual.SundayEnd);
        }
    }
}