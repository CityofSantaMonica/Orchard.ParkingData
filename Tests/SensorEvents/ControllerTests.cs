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
        private static readonly string _utcISO8061BasicFormat = "yyyyMMddTHHmmssZ";
        private static readonly DateTime _referenceDateTime = new DateTime(2015, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        private static readonly SensorEventLifetime _referenceLifetime = new SensorEventLifetime() { Length = 1.0, Units = LifetimeUnits.Hours, Since = _referenceDateTime };

        private SensorEventsController _controller;

        [SetUp]
        public override void TestsSetup()
        {
            base.TestsSetup();

            _controller = new SensorEventsController(_mockSensorEventsService.Object, _mockSiteSevice.Object) {
                Request = _mockRequest,
                RequestContext = _mockRequestContext
            };
        }

        private void setupLifetime()
        {
            _mockSensorEventsService
                .Setup(m => m.GetLifetime())
                .Returns(_referenceLifetime);
        }

        private void setupQuery()
        {
            setupLifetime();

            _mockSensorEventsService
                .Setup(m => m.Query())
                .Returns(
                    new[] {
                        //EventTime in the "future" => should be included in the results
                        new SensorEvent { TransmissionId = 1, EventTime = _referenceDateTime.AddHours(_referenceLifetime.Length * 1) },
                        //EventTime in the "future" => should be included in the results
                        new SensorEvent { TransmissionId = 2, EventTime = _referenceDateTime.AddHours(_referenceLifetime.Length * 2) },
                        //EventTime in the "past" by more than lifetime => should be excluded
                        new SensorEvent { TransmissionId = 3, EventTime = _referenceDateTime.AddHours(_referenceLifetime.Length * -4) }
                    }.AsQueryable()
                );

            _mockSensorEventsService
                .Setup(m => m.ConvertToViewModel(It.IsAny<SensorEvent>()))
                .Returns<SensorEvent>(s => new SensorEventGET() { EventId = s.TransmissionId, EventTime = s.EventTime });
        }

        //nothing
        [TestCase((string)null)]
        [TestCase("")]
        //date no time
        [TestCase("20150101TZ")]
        //time no date
        [TestCase("T000000Z")]
        //date incomplete time
        [TestCase("20150101T00Z")]
        [TestCase("20150101T0000Z")]
        //time incomplete date
        [TestCase("2015T000000Z")]
        [TestCase("201501T000000Z")]
        //no UTC specifier
        [TestCase("20150101T000000")]
        [Category("SensorEvents")]
        public void GetSince_RequiresUTCISO8061_yyyyMMddTHHmmssZ(string argument)
        {
            IHttpActionResult actionResult = _controller.GetSince(argument);
            var contentResult = actionResult as BadRequestErrorMessageResult;

            Assert.IsNotNull(contentResult);
            StringAssert.IsMatch("UTC.+ISO 8061.+basic format", contentResult.Message);
        }

        [Test]
        [Category("SensorEvents")]
        public void GetSince_RequiresArgument_NotBeforeLifetime()
        {
            setupLifetime();

            string argumentBeforeLifetime = _referenceLifetime.Since.AddHours(-1).ToString(_utcISO8061BasicFormat);

            IHttpActionResult actionResult = _controller.GetSince(argumentBeforeLifetime);
            var contentResult = actionResult as BadRequestErrorMessageResult;

            Assert.IsNotNull(contentResult);
            StringAssert.IsMatch("provided.+earlier than.+lifetime", contentResult.Message);
        }

        [Test]
        [Category("SensorEvents")]
        public void GetSince_ReturnsSensorEventGETCollection_WithEventTimeSinceArgument()
        {
            setupQuery();

            string sinceArgument = _referenceLifetime.Since.AddHours(_referenceLifetime.Length / 2).ToString(_utcISO8061BasicFormat);

            IHttpActionResult actionResult = _controller.GetSince(sinceArgument);
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            Assert.IsNull(contentResult.Content.FirstOrDefault(c => c.EventId == 3));
        }

        [Test]
        [Category("SensorEvents")]
        public void GetSince_ReturnsSensorEventGETCollection_WithMostRecentFirst()
        {
            setupQuery();

            string sinceArgument = _referenceLifetime.Since.AddHours(_referenceLifetime.Length / 2).ToString(_utcISO8061BasicFormat);

            IHttpActionResult actionResult = _controller.GetSince(sinceArgument);
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;
            var content = contentResult.Content;

            Assert.Greater(content.First().EventTime, content.Last().EventTime);
        }

        [Test]
        [Category("SensorEvents")]
        public void GetLifetime_ReturnsSensorEventLifetime()
        {
            setupLifetime();

            IHttpActionResult actionResult = _controller.GetLifetime();
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventLifetime>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(_referenceLifetime.Length, contentResult.Content.Length);
            Assert.AreEqual(DateTimeKind.Utc, contentResult.Content.Since.Kind);
            Assert.AreEqual(_referenceLifetime.Since, contentResult.Content.Since);
            Assert.AreEqual(_referenceLifetime.Units, contentResult.Content.Units);
        }

        [Test]
        [Category("SensorEvents")]
        public void GetDefault_ReturnsSensorEventGETCollection_WithEventTimeSinceLifetime()
        {
            setupQuery();

            IHttpActionResult actionResult = _controller.GetDefault();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            Assert.IsNull(contentResult.Content.FirstOrDefault(c => c.EventId == 3));
        }

        [Test]
        [Category("SensorEvents")]
        public void GetDefault_ReturnsSensorEventGETCollection_WithMostRecentFirst()
        {
            setupQuery();

            IHttpActionResult actionResult = _controller.GetDefault();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;
            var content = contentResult.Content;

            Assert.Greater(content.First().EventTime, content.Last().EventTime);
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
