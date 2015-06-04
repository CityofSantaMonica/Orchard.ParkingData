using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using CSM.ParkingData.Controllers;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.Lots
{
    [TestFixture]
    public class ControllerTests
    {
        private Mock<IParkingLotsService> _mockLotService;
        private LotsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockLotService = new Mock<IParkingLotsService>();
            _mockLotService
                .Setup(m => m.Get())
                .Returns(() => new[] { 
                    new ParkingLot { Name = "Lot1", AvailableSpaces = 0 },
                    new ParkingLot { Name = "Lot2", AvailableSpaces = 100 },
                    new ParkingLot { Name = "Structure1", AvailableSpaces = 0 },
                    new ParkingLot { Name = "Structure2", AvailableSpaces = 100 }
                });

            _controller = new LotsController(_mockLotService.Object);
        }

        [Test]
        [Category("Lots")]
        public void Get_ReturnsParkingLotCollection()
        {
            IHttpActionResult actionResult = _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLot>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(4, contentResult.Content.Count());
            Assert.AreEqual(200, contentResult.Content.Aggregate(0, (total, lot) => total += lot.AvailableSpaces));
        }

        [Test]
        [Category("Lots")]
        public void GetWithName_ReturnsParkingLotCollection_FuzzyMatchingName()
        {
            IHttpActionResult actionResult = _controller.Get("lot");
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLot>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            foreach (var result in contentResult.Content)
            {
                StringAssert.IsMatch(@"Lot\d", result.Name);
            }
            Assert.AreEqual(100, contentResult.Content.Aggregate(0, (total, lot) => total += lot.AvailableSpaces));

            actionResult = _controller.Get("struct");
            contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLot>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            foreach (var result in contentResult.Content)
            {
                StringAssert.IsMatch(@"Structure\d", result.Name);
            }
            Assert.AreEqual(100, contentResult.Content.Aggregate(0, (total, lot) => total += lot.AvailableSpaces));
        }
    }
}
