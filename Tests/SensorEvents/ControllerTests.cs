using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using CSM.ParkingData.Controllers;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.SensorEvents
{
    [TestFixture]
    public class ControllerTests
    {
        private HttpRequestMessage _mockRequest;
        private HttpRequestContext _mockRequestContext;
        private Mock<ISensorEventsService> _mockSensorEventService;

        [SetUp]
        public void ControllerTestsSetup()
        {
            _mockSensorEventService = new Mock<ISensorEventsService>();
            
            _mockSensorEventService
                .Setup(m => m.ConvertToViewModel(It.IsAny<SensorEvent>()))
                .Returns<SensorEvent>(
                    se => new SensorEventGET {
                        TransmissionId = se.TransmissionId,
                        MeterId = (se.MeteredSpace ?? new MeteredSpace()).MeterId,
                        SessionId = se.SessionId,
                    }
                );

            _mockRequest = new HttpRequestMessage() {
                Content = new StringContent("StringContent")
            };

            var mockRoute = new Mock<IHttpRoute>();
            mockRoute.SetupGet(m => m.RouteTemplate)
                     .Returns("RouteTemplate");

            var mockRouteData = new Mock<IHttpRouteData>();
            mockRouteData.SetupGet(m => m.Route)
                         .Returns(mockRoute.Object);

            _mockRequestContext = new HttpRequestContext() {
                RouteData = mockRouteData.Object,
            };
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_Returns_SensorEventGETCollection_With_No_Id()
        {
            _mockSensorEventService
                .Setup(m => m.Query())
                .Returns(
                    new[] { 
                        new SensorEvent { TransmissionId = 1, SessionId = 0 },
                        new SensorEvent { TransmissionId = 2, SessionId = 0 },
                        new SensorEvent { TransmissionId = 3, SessionId = 1 }
                    }.AsQueryable()
                );

            var controller = new SensorEventsController(_mockSensorEventService.Object);

            IHttpActionResult actionResult = controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(3, contentResult.Content.Count());
            Assert.AreEqual(2, contentResult.Content.Where(vm => vm.SessionId == 0).Count());
            Assert.AreEqual(1, contentResult.Content.Where(vm => vm.SessionId == 1).Count());
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_Returns_SensorEventGET_With_Id()
        {
            long transmissionId = 42;

            _mockSensorEventService
                .Setup(m => m.Get(transmissionId))
                .Returns(new SensorEvent { TransmissionId = transmissionId });

            var controller = new SensorEventsController(_mockSensorEventService.Object);

            IHttpActionResult actionResult = controller.Get(transmissionId);
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventGET>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(transmissionId, contentResult.Content.TransmissionId);
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_Returns_NotFound_With_Bad_Id()
        {
            var controller = new SensorEventsController(_mockSensorEventService.Object);

            IHttpActionResult actionResult = controller.Get(-100);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void Post_Returns_BadRequest_With_Null_Body()
        {
            var controller = new SensorEventsController(_mockSensorEventService.Object) {
                RequestContext = _mockRequestContext
            };

            IHttpActionResult actionResult = controller.Post(null);

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void Post_Returns_InternalServerError_When_Service_Fails()
        {
            var exception = new Exception("This is the exception mock");

            _mockSensorEventService
                .Setup(m => m.AddOrUpdate(It.IsAny<SensorEventPOST>()))
                .Throws(exception);

            var controller = new SensorEventsController(_mockSensorEventService.Object) {
                Request = _mockRequest,
                RequestContext = _mockRequestContext
            };

            IHttpActionResult actionResult = controller.Post(new SensorEventPOST());

            Assert.IsInstanceOf<ExceptionResult>(actionResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void Post_Returns_Created_SensorEventGET()
        {
            _mockSensorEventService
                .Setup(m => m.AddOrUpdate(It.IsAny<SensorEventPOST>()))
                .Returns<SensorEventPOST>(vm =>
                        new SensorEvent {
                            TransmissionId = long.Parse(vm.TransmissionID),
                            MeteredSpace = new MeteredSpace {
                                MeterId = vm.MeteredSpace.MeterID
                            },
                            SessionId = long.Parse(vm.MeteredSpace.SessionID),
                        }
                );

            var controller = new SensorEventsController(_mockSensorEventService.Object);

            var viewModel = new SensorEventPOST {
                TransmissionID = "1",
                MeteredSpace = new SensorEventMeteredSpacePOST {
                    SessionID = "0",
                    MeterID = "Pole1"
                }
            };

            IHttpActionResult actionResult = controller.Post(viewModel);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<SensorEventGET>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("SensorEvents", createdResult.RouteName);
            Assert.AreEqual(1, createdResult.RouteValues["id"]);

            Assert.IsNotNull(createdResult.Content);
            Assert.AreEqual(1, createdResult.Content.TransmissionId);
            Assert.AreEqual(0, createdResult.Content.SessionId);
            Assert.AreEqual("Pole1", createdResult.Content.MeterId);
        }
    }
}
