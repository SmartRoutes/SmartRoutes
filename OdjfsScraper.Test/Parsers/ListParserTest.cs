﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartRoutes.Model.Odjfs.ChildCareStubs;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Test.Parsers.Support;

namespace SmartRoutes.OdjfsScraper.Test.Parsers
{
    [TestClass]
    public class ListParserTest
    {
        [TestMethod]
        public void HappyPath()
        {
            // ARRANGE
            var template = new ListTemplate();
            var parser = new ListParser();

            // ACT
            IEnumerable<ChildCareStub> actual = parser.Parse(template.GetDocument());

            // ASSERT
            VerifyAreEqual(template.Model, actual);
        }

        public void VerifyAreEqual(IEnumerable<ChildCareStub> expected, IEnumerable<ChildCareStub> actual)
        {
            ChildCareStub[] expectedArray = expected.ToArray();
            ChildCareStub[] actualArray = actual.ToArray();
            Assert.AreEqual(expectedArray.Length, actualArray.Length);
            for (int i = 0; i < expectedArray.Length; i++)
            {
                VerifyAreEqual(expectedArray[i], actualArray[i]);
            }
        }

        public void VerifyAreEqual(ChildCareStub expected, ChildCareStub actual)
        {
            Assert.AreEqual(expected.GetType(), actual.GetType());
            Assert.AreEqual(expected.ExternalUrlId, actual.ExternalUrlId);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.City, actual.City);
        }
    }
}