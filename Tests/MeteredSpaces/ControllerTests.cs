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
    public class ControllerTests : ControllerTestsBase
    {
        [Test]
        [Category("MeteredSpaces")]
        public void Get_Returns_MeteredSpaceGETCollection_With_No_Id()
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

            var controller = new MeteredSpacesController(_mockMeteredSpacesService.Object);

            //act

            IHttpActionResult actionResult = controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<MeteredSpaceGET[]>;

            //assert

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(3, contentResult.Content.Count());
            Assert.AreEqual(2, contentResult.Content.Where(vm => !vm.Active).Count());
            Assert.AreEqual(1, contentResult.Content.Where(vm => vm.Active).Count());
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Get_Returns_MeteredSpaceGET_With_Id()
        {
            //arrange

            string meterId = "Pole1";

            _mockMeteredSpacesService
                .Setup(m => m.Get(meterId))
                .Returns(new MeteredSpace { MeterId = meterId });

            var controller = new MeteredSpacesController(_mockMeteredSpacesService.Object);

            //act

            IHttpActionResult actionResult = controller.Get(meterId);
            var contentResult = actionResult as OkNegotiatedContentResult<MeteredSpaceGET>;

            //assert

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(meterId, contentResult.Content.MeterId);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Get_Returns_NotFound_With_Bad_Id()
        {
            //arrange

            var controller = new MeteredSpacesController(_mockMeteredSpacesService.Object);

            //act

            IHttpActionResult actionResult = controller.Get("bad-id");

            //assert

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Post_Returns_BadRequest_With_Null_Body()
        {
            //arrange

            var controller = new MeteredSpacesController(_mockMeteredSpacesService.Object) {
                RequestContext = _mockRequestContext
            };

            //act

            IHttpActionResult actionResult = controller.Post(null);

            //assert

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Post_Returns_InternalServerError_When_Service_Fails()
        {
            //arrnage

            var exception = new Exception("This is the exception mock");

            _mockMeteredSpacesService
                .Setup(m => m.AddOrUpdate(It.IsAny<MeteredSpacePOST>()))
                .Throws(exception);

            var controller = new MeteredSpacesController(_mockMeteredSpacesService.Object) {
                Request = _mockRequest,
                RequestContext = _mockRequestContext
            };

            //act

            IHttpActionResult actionResult = controller.Post(new[] { new MeteredSpacePOST() });

            //assert

            Assert.IsInstanceOf<ExceptionResult>(actionResult);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void Post_Returns_Created_MeteredSpaceGET()
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

            var controller = new MeteredSpacesController(_mockMeteredSpacesService.Object);

            var viewModel = new MeteredSpacePOST {
                PoleSerialNumber = "Pole1",
                Status = 1
            };

            //act

            IHttpActionResult actionResult = controller.Post(new[] { viewModel });
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<MeteredSpaceGET>;

            //assert

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("MeteredSpaces", createdResult.RouteName);
            Assert.AreEqual("Pole1", createdResult.RouteValues["id"]);

            Assert.IsNotNull(createdResult.Content);
            Assert.AreEqual(true, createdResult.Content.Active);
            Assert.AreEqual("Pole1", createdResult.Content.MeterId);
        }
    }
}
