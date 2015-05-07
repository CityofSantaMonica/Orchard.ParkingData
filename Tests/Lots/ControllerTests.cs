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
    public class ControllerTests
    {
        private Mock<IParkingLotsService> _mockLotService;
        private LotsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockLotService = new Mock<IParkingLotsService>();
            _mockLotService
                .Setup(m => m.Get())
                .Returns(() => new[] { 
                    new ParkingLot { Name = "Lot1", AvailableSpaces = 0 },
                    new ParkingLot { Name = "Lot2", AvailableSpaces = 100 }
                });

            _controller = new LotsController(_mockLotService.Object);
        }

        [Test]
        [Category("Lots")]
        public void Get_Returns_ParkingLotCollection()
        {
            IHttpActionResult actionResult = _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLot>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());

            var fullLot = contentResult.Content.SingleOrDefault(lot => lot.AvailableSpaces == 0);
            var emptyLot = contentResult.Content.SingleOrDefault(lot => lot.AvailableSpaces > 0);

            Assert.NotNull(fullLot);
            Assert.AreEqual("Lot1", fullLot.Name);

            Assert.NotNull(emptyLot);
            Assert.AreEqual("Lot2", emptyLot.Name);
            Assert.AreEqual(100, emptyLot.AvailableSpaces);
        }
    }
}
