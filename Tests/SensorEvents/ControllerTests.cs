using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using CSM.ParkingData.Controllers;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Moq;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.SensorEvents
{
    public class ControllerTests : ControllerTestsBase
    {
        [Test]
        [Category("SensorEvents")]
        public void Get_GivenNoId_ReturnsSensorEventGETCollection()
        {
            _mockSensorEventsService
                .Setup(m => m.Query())
                .Returns(
                    new[] { 
                        new SensorEvent { TransmissionId = 1, SessionId = 0 },
                        new SensorEvent { TransmissionId = 2, SessionId = 0 },
                        new SensorEvent { TransmissionId = 3, SessionId = 1 }
                    }.AsQueryable()
                );

            var controller = new SensorEventsController(_mockSensorEventsService.Object);

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
        public void Get_GivenId_ReturnsSensorEventGET()
        {
            long transmissionId = 42;

            _mockSensorEventsService
                .Setup(m => m.Get(transmissionId))
                .Returns(new SensorEvent { TransmissionId = transmissionId });

            var controller = new SensorEventsController(_mockSensorEventsService.Object);

            IHttpActionResult actionResult = controller.Get(transmissionId);
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventGET>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(transmissionId, contentResult.Content.TransmissionId);
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_GivenBadId_ReturnsNotFound()
        {
            var controller = new SensorEventsController(_mockSensorEventsService.Object);

            IHttpActionResult actionResult = controller.Get(-100);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void Post_GivenNullViewModel_ReturnsBadRequestErrorMessage()
        {
            var controller = new SensorEventsController(_mockSensorEventsService.Object) {
                RequestContext = _mockRequestContext
            };

            IHttpActionResult actionResult = controller.Post(null);

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(actionResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void Post_ReturnsInternalServerError_WhenServiceFails()
        {
            var exception = new Exception("This is the exception mock");

            _mockSensorEventsService
                .Setup(m => m.AddOrUpdate(It.IsAny<SensorEventPOST>()))
                .Throws(exception);

            var controller = new SensorEventsController(_mockSensorEventsService.Object) {
                Request = _mockRequest,
                RequestContext = _mockRequestContext
            };

            IHttpActionResult actionResult = controller.Post(new SensorEventPOST());

            Assert.IsInstanceOf<ExceptionResult>(actionResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void Post_GivenViewModel_ReturnsCreatedSensorEventGETAtRoute()
        {
            _mockSensorEventsService
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

            var controller = new SensorEventsController(_mockSensorEventsService.Object);

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
