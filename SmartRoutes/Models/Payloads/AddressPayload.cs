using System.Linq;

namespace SmartRoutes.Models.Payloads
{
    public class AddressPayload
    {
        public string Address { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string ToString(bool includeAddressLine2)
        {
            string address = Address ?? string.Empty;
            if (includeAddressLine2 && !string.IsNullOrEmpty(AddressLine2))
            {
                address = (address + " " + AddressLine2).TrimEnd();
            }

            return string.Join(", ", new[]
            {
                address,
                City ?? string.Empty,
                State ?? string.Empty,
                ZipCode ?? string.Empty
            }.Where(s => !string.IsNullOrEmpty(s)));
        }

        public override string ToString()
        {
            return ToString(false);
        }
    }
}