namespace SmartRoutes.Models.Itinerary
{
    public interface IAddressAction : IItineraryAction
    {
        /// <summary>
        /// The address from which to depart for this asction.
        /// </summary>
        string Address { get; set; }
    }
}