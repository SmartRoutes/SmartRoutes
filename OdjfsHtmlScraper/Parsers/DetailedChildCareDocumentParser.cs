using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Model.Odjfs;
using NLog;
using Scraper;

namespace OdjfsHtmlScraper.Parsers
{
    public class DetailedChildCareDocumentParser<T> : ChildCareDocumentParser<T> where T : DetailedChildCare
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override T PopulateFields(T childCare, IDictionary<string, string> details)
        {
            // populate the base fields
            base.PopulateFields(childCare, details);

            // program details table
            childCare.Address = GetDetailString(details, "Address");
            childCare.CenterStatus = GetDetailString(details, "Center Status");
            childCare.Administrators = GetDetailString(details, "Administrator", "Administrators");
            childCare.ProviderAgreement = GetDetailString(details, "Provider Agreement");
            childCare.InitialApplicationDate = GetDetailString(details, "Initial Application Date");
            childCare.LicenseBeginDate = GetDetailString(details, "License Begin Date");
            childCare.LicenseEndDate = GetDetailString(details, "License Expiration Date");
            childCare.PhoneNumber = GetDetailString(details, "Phone");

            string sutqRating = GetDetailString(details, "SUTQ Rating");
            childCare.SutqRating = sutqRating != null ? details["SUTQ Rating"].Length : (int?) null;

            // children served column
            childCare.Infants = GetDetailBool(details, "Infants");
            childCare.YoungToddlers = GetDetailBool(details, "Younger Toddler");
            childCare.OlderToddlers = GetDetailBool(details, "Older Toddler");
            childCare.Preschoolers = GetDetailBool(details, "Preschool");
            childCare.Gradeschoolers = GetDetailBool(details, "School Age");
            childCare.ChildCareFoodProgram = GetDetailBool(details, "Child Care");

            // accreditations column
            childCare.Naeyc = GetDetailBool(details, "NAEYC");
            childCare.Necpa = GetDetailBool(details, "NECPA");
            childCare.Naccp = GetDetailBool(details, "NACCP");
            childCare.Naccp = GetDetailBool(details, "NAFCC");
            childCare.Coa = GetDetailBool(details, "COA");
            childCare.Acsi = GetDetailBool(details, "ACSI");

            // Monday
            Tuple<bool, DateTime?, DateTime?> monday = GetHoursOfOperation(details, "Monday");
            childCare.MondayReported = monday.Item1;
            childCare.MondayBegin = monday.Item2;
            childCare.MondayEnd = monday.Item3;

            // Tuesday
            Tuple<bool, DateTime?, DateTime?> tuesday = GetHoursOfOperation(details, "Tuesday");
            childCare.TuesdayReported = tuesday.Item1;
            childCare.TuesdayBegin = tuesday.Item2;
            childCare.TuesdayEnd = tuesday.Item3;

            // Wednesday
            Tuple<bool, DateTime?, DateTime?> wednesday = GetHoursOfOperation(details, "Wednesday");
            childCare.WednesdayReported = wednesday.Item1;
            childCare.WednesdayBegin = wednesday.Item2;
            childCare.WednesdayEnd = wednesday.Item3;

            // Thursday
            Tuple<bool, DateTime?, DateTime?> thursday = GetHoursOfOperation(details, "Thursday");
            childCare.ThursdayReported = thursday.Item1;
            childCare.ThursdayBegin = thursday.Item2;
            childCare.ThursdayEnd = thursday.Item3;

            // Friday
            Tuple<bool, DateTime?, DateTime?> friday = GetHoursOfOperation(details, "Friday");
            childCare.FridayReported = friday.Item1;
            childCare.FridayBegin = friday.Item2;
            childCare.FridayEnd = friday.Item3;

            // Saturday
            Tuple<bool, DateTime?, DateTime?> saturday = GetHoursOfOperation(details, "Saturday");
            childCare.SaturdayReported = saturday.Item1;
            childCare.SaturdayBegin = saturday.Item2;
            childCare.SaturdayEnd = saturday.Item3;

            // Sunday
            Tuple<bool, DateTime?, DateTime?> sunday = GetHoursOfOperation(details, "Sunday");
            childCare.SundayReported = sunday.Item1;
            childCare.SundayBegin = sunday.Item2;
            childCare.SundayEnd = sunday.Item3;

            return childCare;
        }

        private Tuple<bool, DateTime?, DateTime?> GetHoursOfOperation(IDictionary<string, string> details, string key)
        {
            string value = GetDetailString(details, key);
            if (value == "NOT REPORTED")
            {
                return new Tuple<bool, DateTime?, DateTime?>(false, null, null);
            }
            if (value == "CLOSED")
            {
                return new Tuple<bool, DateTime?, DateTime?>(true, null, null);
            }

            // parse the string
            Match match = Regex.Match(value, @"^(?<BeginHour>\d{2}):(?<BeginMinute>\d{2}) (?<BeginPeriod>AM|PM) to (?<EndHour>\d{2}):(?<EndMinute>\d{2}) (?<EndPeriod>AM|PM)$");
            if (!match.Success)
            {
                var exception = new ParserException("An hours of operation string was not in an expected format.");
                Logger.ErrorException(string.Format("Key: '{0}', Value: '{1}'", key, value), exception);
                throw exception;
            }

            // create DateTime instances
            DateTime beginTime = new DateTime(1970, 1, 1)
                .AddHours(int.Parse(match.Groups["BeginHour"].Value))
                .AddMinutes(int.Parse(match.Groups["BeginMinute"].Value))
                .AddHours(match.Groups["BeginPeriod"].Value == "PM" ? 12 : 0);

            DateTime endTime = new DateTime(1970, 1, 1)
                .AddHours(int.Parse(match.Groups["EndHour"].Value))
                .AddMinutes(int.Parse(match.Groups["EndMinute"].Value))
                .AddHours(match.Groups["EndPeriod"].Value == "PM" ? 12 : 0);

            if (beginTime >= endTime)
            {
                var exception = new ParserException("An hours of operation string has a begin time equal to or after an end time.");
                Logger.ErrorException(string.Format("Key: '{0}', Value: '{1}', BeginTime: '{2}', EndTime: '{3}'", key, value, beginTime, endTime), exception);
                throw exception;
            }

            return new Tuple<bool, DateTime?, DateTime?>(true, beginTime, endTime);
        }

        private bool GetDetailBool(IDictionary<string, string> details, params string[] keys)
        {
            string value = GetDetailString(details, keys);
            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }

            var exception = new ParserException("A boolean detail could not be parsed.");
            Logger.ErrorException(string.Format("Keys: '{0}', Value: '{1}'", string.Join(", ", keys), value), exception);
            throw exception;
        }
    }
}