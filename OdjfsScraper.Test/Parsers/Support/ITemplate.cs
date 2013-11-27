namespace SmartRoutes.OdjfsScraper.Test.Parsers.Support
{
    public interface ITemplate<out T>
    {
        T Model { get; }
        byte[] GetDocument();
    }
}