namespace BundleCounter.Models.Requests
{
    public class BundleData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Amount { get; set; }
        public int Need { get; set; }
        public int ParentId { get; set; }
    }
}
