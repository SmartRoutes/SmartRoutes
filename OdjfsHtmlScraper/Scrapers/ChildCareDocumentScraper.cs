﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsQuery;
using Model.Odjfs;
using NLog;
using OdjfsHtmlScraper.Support;
using Scraper;

namespace OdjfsHtmlScraper.Scrapers
{
    public class ChildCareDocumentScraper : IScraper<ChildCare, ChildCare>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IClient _client;
        private readonly IDictionary<Type, IParser<CQ, ChildCare>> _parsers;

        public ChildCareDocumentScraper(IClient client, IParser<CQ, TypeAHome> typeAHomeParser, IParser<CQ, TypeBHome> typeBHomeParser, IParser<CQ, LicensedCenter> licensedCenterParser, IParser<CQ, DayCamp> dayCampParser)
        {
            _client = client;

            // store the parsers in a dictionary for quick lookup
            _parsers = new Dictionary<Type, IParser<CQ, ChildCare>>
            {
                {typeof (TypeAHome), typeAHomeParser},
                {typeof (TypeBHome), typeBHomeParser},
                {typeof (LicensedCenter), licensedCenterParser},
                {typeof (DayCamp), dayCampParser},
            };
        }

        public async Task<ChildCare> Scrape(ChildCare childCare)
        {
            // make sure we have a parser before we fetch the HTML
            Type childCareType = childCare.GetType();
            if (!_parsers.ContainsKey(childCareType))
            {
                var exception = new ArgumentException("No parser was found for the provided child care type.");
                Logger.ErrorException(string.Format("SupportedTypes: '{0}', ProvidedType: '{1}'", string.Join(", ", _parsers.Keys.Select(t => t.ToString())), childCare), exception);
                throw exception;
            }
            IParser<CQ, ChildCare> parser = _parsers[childCareType];

            // make sure we have a URL ID
            if (string.IsNullOrWhiteSpace(childCare.ExternalUrlId))
            {
                var exception = new ArgumentNullException("childCare", "The provided child care has a null external URL ID.");
                Logger.ErrorException(string.Format("SupportedTypes: '{0}', ProvidedType: '{1}'", string.Join(", ", _parsers.Keys.Select(t => t.ToString())), childCare), exception);
                throw exception;
            }

            // fetch and parse the HTML
            CQ document = await _client.GetChildCareDocument(childCare);

            // TODO: verify child care argument matches the extracted child care

            // extract the child care information
            return parser.Parse(document);
        }
    }
}