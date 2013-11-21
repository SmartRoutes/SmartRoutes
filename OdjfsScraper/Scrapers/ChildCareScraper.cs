﻿using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SmartRoutes.Model.Odjfs.ChildCares;
using SmartRoutes.Model.Odjfs.ChildCareStubs;
using NLog;
using SmartRoutes.OdjfsScraper.Parsers;
using SmartRoutes.OdjfsScraper.Support;
using SmartRoutes.Scraper;

namespace SmartRoutes.OdjfsScraper.Scrapers
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
            ClientResponse response = await _odjfsClient.GetChildCareDocument(childCareStub);
            ValidateClientResponse(response);
            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }

            // extract the child care information
            return _parser.Parse(childCareStub, response.Content);
        }

        public async Task<ChildCare> Scrape(ChildCare childCare)
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
            ClientResponse response = await _odjfsClient.GetChildCareDocument(childCare);
            ValidateClientResponse(response);
            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }

            // extract the child care information
            return _parser.Parse(childCare, response.Content);
        }

        private void ValidateClientResponse(ClientResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK && // the record will be updated (with new parse)
                response.StatusCode != HttpStatusCode.NotFound && // the record will be deleted
                response.StatusCode != HttpStatusCode.InternalServerError) // the record will be deleted
            {
                var exception = new ScraperException("A status code that is not 200 or 404 was returned when getting a child care document.");
                Logger.ErrorException(string.Format(
                    "RequestUri: '{0}', StatusCode: '{1}'",
                    response.RequestUri,
                    response.StatusCode), exception);
                throw exception;
            }
        }
    }
}