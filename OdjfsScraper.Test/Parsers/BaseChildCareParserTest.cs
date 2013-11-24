using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Test.Parsers.Support;
using SmartRoutes.Scraper;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    public abstract class BaseChildCareParserTest<TModel, TTemplate, TParser>
        where TModel : ChildCare
        where TTemplate : ChildCareTemplate<TModel>
        where TParser : BaseChildCareParser<TModel>
    {
        public virtual void HappyPath()
        {
            // ARRANGE
            var template = Activator.CreateInstance<TTemplate>();
            var parser = Activator.CreateInstance<TParser>();
            var actualModel = Activator.CreateInstance<TModel>();

            // ACT
            actualModel = parser.Parse(actualModel, template.GetDocument());

            // ASSERT
            VerifyAreEqual(template.Model, actualModel);
        }

        public virtual void MissingDetail()
        {
            // ARRANGE
            var template = Activator.CreateInstance<TTemplate>();
            var parser = Activator.CreateInstance<TParser>();

            foreach (var detail in template.Details.ToArray())
            {
                template.Details.Remove(detail.Key);
                try
                {
                    // ACT
                     parser.Parse(Activator.CreateInstance<TModel>(), template.GetDocument());

                    // ASSERT
                    Assert.Fail("When detail {0} is missing, an exception should be thrown.", detail.Key);
                }
                catch (ParserException)
                {
                    template.Details.Add(detail);
                }
            }
        }

        public virtual void IgnoreSpacerImages()
        {
            // ARRANGE
            var template = Activator.CreateInstance<TTemplate>();
            var parser = Activator.CreateInstance<TParser>();
            var actualModel = Activator.CreateInstance<TModel>();

            // ACT
            actualModel = parser.Parse(actualModel, template.GetDocument());

            // ASSERT
            VerifyAreEqual(template.Model, actualModel);
        }

        protected virtual void VerifyAreEqual(TModel expected, TModel actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.ExternalId, actual.ExternalId);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.City, actual.City);
            Assert.AreEqual(expected.State, actual.State);
            Assert.AreEqual(expected.ZipCode, actual.ZipCode);
            Assert.AreEqual(expected.County.Name, actual.County.Name);
            Assert.AreEqual(expected.PhoneNumber, actual.PhoneNumber);
        }
    }
}