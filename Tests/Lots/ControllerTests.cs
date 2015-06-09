using System;
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
                    new ParkingLotGET { Name = "Lot 1", AvailableSpaces = 0 },
                    new ParkingLotGET { Name = "Lot 2", AvailableSpaces = 100 },
                    new ParkingLotGET { Name = "Structure 1", AvailableSpaces = 0 },
                    new ParkingLotGET { Name = "Structure 2", AvailableSpaces = 100 }
                });

            _controller = new LotsController(_mockLotService.Object);
        }

        [Test]
        [Category("Lots")]
        public void GetDefault_ReturnsParkingLotCollection()
        {
            IHttpActionResult actionResult = _controller.GetDefault();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLotGET>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(4, contentResult.Content.Count());
            Assert.AreEqual(200, contentResult.Content.Aggregate(0, (total, lot) => total += lot.AvailableSpaces));
        }

        [Test]
        [Category("Lots")]
        public void GetDefault_WithId_ReturnsParkingLotWithId()
        {
            //the id for Lot 1
            long id = 7649;

            IHttpActionResult actionResult = _controller.GetDefault(id);
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLotGET>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(1, contentResult.Content.Count());
            Assert.AreEqual(id, contentResult.Content.Single().Id);
        }

        [TestCase((string)null)]
        [TestCase("")]
        [Category("Lots")]
        public void GetMatching_WithoutName_ReturnsBadRequest(string badName)
        {
            IHttpActionResult actionResult = _controller.GetMatching(badName);
            var contentResult = actionResult as BadRequestErrorMessageResult;

            Assert.NotNull(contentResult);
        }

        [Test]
        [Category("Lots")]
        public void GetMatching_WithName_ReturnsParkingLotCollection_FuzzyMatchingName()
        {
            IHttpActionResult actionResult = _controller.GetMatching("lot");
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLotGET>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            foreach (var result in contentResult.Content)
            {
                StringAssert.IsMatch(@"Lot \d", result.Name);
            }
            Assert.AreEqual(100, contentResult.Content.Aggregate(0, (total, lot) => total += lot.AvailableSpaces));

            actionResult = _controller.GetMatching("struct");
            contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<ParkingLotGET>>;

            Assert.NotNull(contentResult);
            Assert.NotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            foreach (var result in contentResult.Content)
            {
                StringAssert.IsMatch(@"Structure \d", result.Name);
            }
            Assert.AreEqual(100, contentResult.Content.Aggregate(0, (total, lot) => total += lot.AvailableSpaces));
        }
    }
}
