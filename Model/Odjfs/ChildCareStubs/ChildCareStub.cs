namespace Model.Odjfs.ChildCareStubs
{
    public abstract class ChildCareStub
    {
        public int Id { get; set; }
        public abstract string ChildCareType { get; }

        #region HTML

        public string ExternalUrlId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        #endregion
    }
}