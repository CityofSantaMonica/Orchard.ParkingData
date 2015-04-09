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
using Orchard.Services;

namespace CSM.ParkingData.Tests.SensorEvents
{
    public class ControllerTests : ControllerTestsBase
    {
        private static DateTime _referenceDateTime = new DateTime(2015, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        private SensorEventsController _controller;
        private Mock<IClock> _mockClock;

        [SetUp]
        public override void TestsSetup()
        {
            base.TestsSetup();

            _mockClock = new Mock<IClock>();
            _mockClock
                .Setup(m => m.UtcNow)
                .Returns(_referenceDateTime);

            _controller = new SensorEventsController(_mockClock.Object, _mockSensorEventsService.Object, _mockSiteSevice.Object) {
                Request = _mockRequest,
                RequestContext = _mockRequestContext
            };
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_GivenNoId_ReturnsSensorEventGETCollection_WithEventTimeSinceTimeLimitHoursBeforeUtcNow()
        {
            SensorEventLifetime lifetime = new SensorEventLifetime() { Length = 1.0, Scope = LifetimeScope.Hours };

            _mockSensorEventsService
                .Setup(m => m.Query())
                .Returns(
                    new[] {
                        //EventTime in the "future" => should be included in the results
                        new SensorEvent { TransmissionId = 1, EventTime = _referenceDateTime.AddHours(lifetime.Length * 1) },
                        //EventTime in the "future" => should be included in the results
                        new SensorEvent { TransmissionId = 2, EventTime = _referenceDateTime.AddHours(lifetime.Length * 2) },
                        //EventTime in the "past" by more than lifeTimeHours => should be excluded
                        new SensorEvent { TransmissionId = 3, EventTime = _referenceDateTime.AddHours(lifetime.Length * -2) }
                    }.AsQueryable()
                );

            _mockSensorEventsService
                .Setup(m => m.GetLifetime())
                .Returns(lifetime);

            IHttpActionResult actionResult = _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            Assert.IsNull(contentResult.Content.FirstOrDefault(c => c.EventId == 3));
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_GivenId_ReturnsSensorEventGET()
        {
            long transmissionId = 42;

            _mockSensorEventsService
                .Setup(m => m.Get(transmissionId))
                .Returns(new SensorEvent { TransmissionId = transmissionId });

            IHttpActionResult actionResult = _controller.Get(transmissionId);
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventGET>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(transmissionId, contentResult.Content.EventId);
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_GivenBadId_ReturnsNotFound()
        {
            IHttpActionResult actionResult = _controller.Get(-100);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void Post_GivenNullViewModel_ReturnsBadRequestErrorMessage()
        {
            IHttpActionResult actionResult = _controller.Post(null);

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

            IHttpActionResult actionResult = _controller.Post(new SensorEventPOST());

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

            var viewModel = new SensorEventPOST {
                TransmissionID = "1",
                MeteredSpace = new SensorEventMeteredSpacePOST {
                    SessionID = "0",
                    MeterID = "Pole1"
                }
            };

            IHttpActionResult actionResult = _controller.Post(viewModel);
            var okResult = actionResult as OkNegotiatedContentResult<SensorEventGET>;

            Assert.IsNotNull(okResult);
            //Assert.AreEqual("SensorEvents", createdResult.RouteName);
            //Assert.AreEqual(1, createdResult.RouteValues["id"]);

            Assert.IsNotNull(okResult.Content);
            Assert.AreEqual(1, okResult.Content.EventId);
            Assert.AreEqual(0, okResult.Content.SessionId);
            Assert.AreEqual("Pole1", okResult.Content.MeterId);
        }
    }
}
