namespace SmartRoutes.Model.Srds
{
    public class AttributeValue
    {
        public int Id { get; set; }
        public int AttributeKeyId { get; set; }
        public virtual AttributeKey AttributeKey { get; set; }
        public int DestinationId { get; set; }
        public virtual Destination Destination { get; set; }
        public object Value { get; set; }
        public string ValueString { get; set; }
    }
}