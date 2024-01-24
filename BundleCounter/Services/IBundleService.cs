using BundleCounter.Models.Requests;
using BundleCounter.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BundleCounter.Services
{
    public interface IBundleService
    {
        ProcResult<int> GetMaxBundleCount(BundleCountRequest req);
    }
}
