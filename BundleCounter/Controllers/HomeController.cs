using BundleCounter.Models;
using BundleCounter.Models.Requests;
using BundleCounter.Models.Responses;
using BundleCounter.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BundleCounter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBundleService bundleService;

        public HomeController(ILogger<HomeController> logger, IBundleService bundleService)
        {
            _logger = logger;

            this.bundleService = bundleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        [Route("bundlecount")]
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