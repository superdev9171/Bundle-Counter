namespace BundleCounter.Models.Requests
{
    public class BundleCountRequest
    {
        public int BundleId { get; set; }
        public List<BundleData>? Bundles { get; set; }
    }
}
