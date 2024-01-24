using BundleCounter;
using BundleCounter.Models.Requests;
using BundleCounter.Models.Responses;
using BundleCounter.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework.Internal;

namespace BundleCounterTest
{
    public class BundleTest
    {
        public ServiceProvider serviceProvider;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AppDBContext>();
            services.AddTransient<IBundleService, BundleService>();

            serviceProvider = services.BuildServiceProvider();
        }

        [Test]
        public void TestBicycle()
        {
            var bundleService = serviceProvider.GetService<IBundleService>();

            Assert.IsNotNull(bundleService, "Failed to get bundle service.");

            var res = bundleService.GetMaxBundleCount(new BundleCountRequest
            {
                BundleId = 1,
                Bundles = new List<BundleData>()
                {
                    new BundleData { Id = 1, Name = "Bicycle",  ParentId = -1 },
                    new BundleData { Id = 2, Name = "Seat",     ParentId = 1,   Need = 1, Amount = 50},
                    new BundleData { Id = 3, Name = "Pedal",    ParentId = 1,   Need = 2, Amount = 60 },
                    new BundleData { Id = 4, Name = "Wheel",    ParentId = 1,   Need = 2 },
                    new BundleData { Id = 5, Name = "Frame",    ParentId = 4,   Need = 1, Amount = 60 },
                    new BundleData { Id = 6, Name = "Tube",     ParentId = 4,   Need = 1, Amount = 35 },
                }
            });

            Assert.IsTrue(res.Success, "Failed to get max bundle count.");
            Assert.That(res.Data, Is.EqualTo(17), "The result is wrong");
        }
    }
}