using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using CSM.ParkingData.Controllers;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Moq;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.MeteredSpaces
{
    [TestFixture]
    public class ControllerTests : ControllerTestsBase
    {
        private MeteredSpacesController _controller;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _controller = new MeteredSpacesController(_mockMeteredSpacesService.Object) {
                Request = _requestStub,
                RequestContext = _requestContextStub
            };
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Get_GivenNoId_ReturnsMeteredSpaceGETCollection()
        {
            //arrange

            _mockMeteredSpacesService
                .Setup(m => m.Query())
                .Returns(
                    new[] { 
                        new MeteredSpace { MeterId = "Pole1", Active = false },
                        new MeteredSpace { MeterId = "Pole2", Active = false },
                        new MeteredSpace { MeterId = "Pole3", Active = true }
                    }.AsQueryable()
                );

            //act

            IHttpActionResult actionResult = _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<MeteredSpaceGET[]>;

            //assert

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(3, contentResult.Content.Count());
            Assert.AreEqual(2, contentResult.Content.Where(vm => !vm.Active.Value).Count());
            Assert.AreEqual(1, contentResult.Content.Where(vm => vm.Active.Value).Count());
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Get_GivenId_ReturnsMeteredSpaceGET()
        {
            //arrange

            string meterId = "Pole1";

            _mockMeteredSpacesService
                .Setup(m => m.Get(meterId))
                .Returns(new MeteredSpace { MeterId = meterId });

            //act

            IHttpActionResult actionResult = _controller.Get(meterId);
            var contentResult = actionResult as OkNegotiatedContentResult<MeteredSpaceGET>;

            //assert

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(meterId, contentResult.Content.MeterId);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Get_GivenBadId_ReturnsNotFound()
        {
            //act

            IHttpActionResult actionResult = _controller.Get("bad-id");

            //assert

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Post_GivenNullViewModel_ReturnsBadRequest()
        {
            //act

            IHttpActionResult actionResult = _controller.Post(null);

            //assert

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Post_GivenEmptyViewModel_ReturnsBadRequest()
        {
            var emptyViewModelCollection = new MeteredSpacePOSTCollection();

            //act

            IHttpActionResult actionResult = _controller.Post(emptyViewModelCollection);

            //assert

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Post_ReturnsInternalServerError_WhenServiceFails()
        {
            //arrnage

            var exception = new Exception("This is the exception mock");

            _mockMeteredSpacesService
                .Setup(m => m.AddOrUpdate(It.IsAny<MeteredSpacePOST>()))
                .Throws(exception);

            var viewModelCollection = new MeteredSpacePOSTCollection { new MeteredSpacePOST() };

            //act

            IHttpActionResult actionResult = _controller.Post(viewModelCollection);

            //assert

            Assert.IsInstanceOf<ExceptionResult>(actionResult);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Post_GivenViewModel_ReturnsCreatedMeteredSpaceGETAtRoute()
        {
            //arrange

            _mockMeteredSpacesService
                .Setup(m => m.AddOrUpdate(It.IsAny<MeteredSpacePOST>()))
                .Returns<MeteredSpacePOST>(vm =>
                    new MeteredSpace {
                        MeterId = vm.PoleSerialNumber,
                        Active = vm.Status == 1
                    }
                );

            var viewModelCollection = new MeteredSpacePOSTCollection {
                new MeteredSpacePOST {
                    PoleSerialNumber = "Pole1",
                    Status = 1
                }
            };

            //act

            IHttpActionResult actionResult = _controller.Post(viewModelCollection);
            var okResult = actionResult as OkNegotiatedContentResult<MeteredSpaceGET>;

            //assert

            Assert.IsNotNull(okResult);
            //Assert.AreEqual("MeteredSpaces", createdResult.RouteName);
            //Assert.AreEqual("Pole1", createdResult.RouteValues["id"]);

            Assert.IsNotNull(okResult.Content);
            Assert.AreEqual(true, okResult.Content.Active);
            Assert.AreEqual("Pole1", okResult.Content.MeterId);
        }
    }
}
