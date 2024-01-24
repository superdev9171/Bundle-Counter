using BundleCounter.Models;
using BundleCounter.Models.Requests;
using BundleCounter.Models.Responses;
using BundleCounter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BundleCounter.Controllers
{
    [Route("/bundle")]
    public class BundleController : Controller
    {
        private readonly ILogger<BundleController> _logger;
        private readonly IBundleService bundleService;

        public BundleController(ILogger<BundleController> logger, IBundleService bundleService)
        {
            _logger = logger;

            this.bundleService = bundleService;
        }

        [HttpGet]
        public ProcResult<List<BundleData>> GetBundles()
        {
            return bundleService.GetBundles();
        }

        [HttpPost]
        public ProcResult<bool> SaveBundles([FromForm] BundleSaveRequest req)
        {
            return bundleService.SaveBundles(req);
        }

        [Route("count")]
        [HttpPost]
        public ProcResult<int> GetMaxBundleCount([FromForm] BundleCountRequest req)
        {
            return bundleService.GetMaxBundleCount(req);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}