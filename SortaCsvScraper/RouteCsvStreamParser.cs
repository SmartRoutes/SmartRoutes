using System.Collections.Generic;
using Model.Sorta;
using Scraper;

namespace SortaCsvScraper
{
    public class RouteCsvStreamParser : CsvStreamParser<Route>
    {
        protected override Route ConstructItem(IDictionary<string, string> values)
        {
            return new Route
            {
                Id = int.Parse(values["route_id"]),
                AgencyId = values["agency_id"],
                ShortName = values["route_short_name"],
                LongName = values["route_long_name"],
                Description = values["route_desc"],
                RouteTypeId = int.Parse(values["route_type"]),
                Url = values["route_url"],
                Color = values["route_color"],
                TextColor = values["route_text_color"]
            };
        }
    }
}