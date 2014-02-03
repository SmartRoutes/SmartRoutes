namespace SmartRoutes.Model.Srds
{
    public interface IDestination : ILocation
    {
        string Name { get; set; }
    }
}