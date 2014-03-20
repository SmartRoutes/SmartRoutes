namespace SmartRoutes.Models.Map
{
    public class StopTimeLocationModel : TimeLocationModel
    {
        public string Route { get; set; }
        public string Stop { get; set; }
    }
}