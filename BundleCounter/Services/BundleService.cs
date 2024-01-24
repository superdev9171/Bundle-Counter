using BundleCounter.Controllers;
using BundleCounter.Models.Requests;
using BundleCounter.Models.Responses;
using System.Linq.Expressions;

namespace BundleCounter.Services
{
    public class BundleService : IBundleService
    {
        private readonly ILogger<BundleService> _logger;

        public BundleService(ILogger<BundleService> logger) 
        { 
            _logger = logger;
        }

        public ProcResult<int> GetMaxBundleCount(BundleCountRequest req)
        {
            var res = new ProcResult<int>();

            try
            {
                var bundles = GetBundles(req.Bundles);
                var root = bundles.FirstOrDefault(bd => bd.Id == req.BundleId);
                if (root == null)
                {
                    res.ErrorMessage = "Failed to find the bundle in the list.";
                    return res;
                }

                res.Success = true;
                res.Data = GetMaxCount(root);

                return res;
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }

            return res;
        }

        private List<Bundle> GetBundles(List<BundleData>? data)
        {
            var bundles = new List<Bundle>();
            
            if (data == null)
                return bundles;

            foreach(var item in data)
            {
                var bundle = new Bundle
                {
                    Id = item.Id,
                    Name = item.Name,
                    Need = item.Need,
                    Amount = item.Amount,
                    ParentId = item.ParentId,
                };

                var parent = bundles.FirstOrDefault(bd => bd.Id == bundle.ParentId);
                
                if (parent != null)
                {
                    bundle.Parent = parent;
                    if (parent.Parts != null)
                        parent.Parts.Add(bundle);
                    else
                        parent.Parts = new List<Bundle>() { bundle };
                }    

                bundles.Add(bundle);
            }

            return bundles;
        }

        private int GetMaxCount(Bundle bundle)
        {
            int max = -1;

            if (bundle.Parts == null)
            {
                if (bundle.Parent == null)
                    return -1;

                return bundle.Amount;
            }

            foreach(var part in bundle.Parts)
            {
                int partCount = GetMaxCount(part);
                int count = partCount / part.Need;

                max = max == -1 ? count : max = Math.Min(max, part.Amount);
            }

            return max;
        }
    }
}
