using BundleCounter.Models.Requests;
using BundleCounter.Models.Responses;
using BundleCounter.Repository;

namespace BundleCounter.Services
{
    public class BundleService : IBundleService
    {
        private readonly ILogger<BundleService> _logger;
        private readonly BundleRepository bundleRepository;

        public BundleService(AppDBContext appDBContext, ILogger<BundleService> logger) 
        { 
            _logger = logger;

            bundleRepository = new BundleRepository(appDBContext);
        }

        public ProcResult<int> GetMaxBundleCount(BundleCountRequest req)
        {
            var res = new ProcResult<int>();

            try
            {
                var bundles = GetBundlesFromData(req.Bundles);
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

        private List<Bundle> GetBundlesFromData(List<BundleData>? data)
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

                max = max == -1 ? count : Math.Min(max, count);
            }

            return max;
        }

        public ProcResult<bool> SaveBundles(BundleSaveRequest req)
        {
            var res = new ProcResult<bool>();

            try
            {
                var bundles = GetBundlesFromData(req.Bundles);

                // Remove existing bundles
                var existingBundles = bundleRepository.FindAll();
                bundleRepository.RemoveAll(existingBundles);

                // Add new bundles
                foreach(var bundle in bundles)
                {
                    bundle.ParentId = null;
                }

                bundleRepository.Insert(bundles[0]);

                res.Success = true;

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }

            return res;
        }

        public ProcResult<List<BundleData>> GetBundles()
        {
            var res = new ProcResult<List<BundleData>>();

            try
            {
                var bundles = bundleRepository.FindAll();

                var list = bundles.Select(bd => new BundleData
                {
                    Id = bd.Id,
                    ParentId = bd.ParentId ?? -1,
                    Name = bd.Name,
                    Amount = bd.Amount,
                    Need = bd.Need
                }).ToList();

                res.Success = true;
                res.Data = list;

                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }

            return res;
        }
    }
}
