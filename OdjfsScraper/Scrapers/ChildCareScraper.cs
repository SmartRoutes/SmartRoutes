﻿using System;
using System.Threading.Tasks;
using Model.Odjfs.ChildCares;
using Model.Odjfs.ChildCareStubs;
using NLog;
using OdjfsScraper.Parsers;
using OdjfsScraper.Support;

namespace OdjfsScraper.Scrapers
{
    public class ChildCareScraper : IChildCareScraper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IOdjfsClient _odjfsClient;
        private readonly IChildCareParser _parser;

        public ChildCareScraper(IOdjfsClient odjfsClient, IChildCareParser parser)
        {
            _odjfsClient = odjfsClient;
            _parser = parser;
        }

        public async Task<ChildCare> Scrape(ChildCareStub childCareStub)
        {
            // make sure we have a URL ID
            if (string.IsNullOrWhiteSpace(childCareStub.ExternalUrlId))
            {
                var exception = new ArgumentNullException("childCareStub", "The provided child care stub has a null external URL ID.");
                Logger.ErrorException(string.Format(
                    "Type: '{0}', ExternalUrlId: '{1}'",
                    childCareStub.GetType(),
                    childCareStub.ExternalUrlId), exception);
                throw exception;
            }

            // fetch the contents
            byte[] bytes = await _odjfsClient.GetChildCareDocument(childCareStub);

            // extract the child care information
            return _parser.Parse(childCareStub, bytes);
        }

        public async Task Scrape(ChildCare childCare)
        {
            // make sure we have a URL ID
            if (string.IsNullOrWhiteSpace(childCare.ExternalUrlId))
            {
                var exception = new ArgumentNullException("childCare", "The provided child care has a null external URL ID.");
                Logger.ErrorException(string.Format(
                    "Type: '{0}', ExternalUrlId: '{1}'",
                    childCare.GetType(),
                    childCare.ExternalUrlId), exception);
                throw exception;
            }

            // fetch the contents
            byte[] bytes = await _odjfsClient.GetChildCareDocument(childCare);

            // extract the child care information
            _parser.Parse(childCare, bytes);
        }
    }
}