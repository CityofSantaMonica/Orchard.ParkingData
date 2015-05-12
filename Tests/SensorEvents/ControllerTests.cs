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
    [TestFixture]
    public class ControllerTests : ControllerTestsBase
    {
        private SensorEventsController _controller;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _controller = new SensorEventsController(_mockMeteredSpacesService.Object, _mockSensorEventsService.Object) {
                Request = _requestStub,
                RequestContext = _requestContextStub
            };
        }

        private void anyMeterIdExists()
        {
            _mockMeteredSpacesService
                .Setup(m => m.Exists(It.IsAny<string>()))
                .Returns(true);
        }

        //nothing
        [TestCase((string)null)]
        [TestCase("")]
        //not even close
        [TestCase("not a date")]
        [TestCase("January 1, 2015 12:00 A.M.")]
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
        public void EvaluateRequestedDateTime_RequiresArgument_InExpectedFormat(string argument)
        {
            IHttpActionResult badRequest = null;
            DateTime parsed;

            bool success = _controller.EvaluateRequestedDateTime(argument, out parsed, out badRequest);

            Assert.IsFalse(success);
            Assert.IsNotNull(badRequest as BadRequestErrorMessageResult);
            StringAssert.IsMatch("UTC.+ISO 8061.+basic format", (badRequest as BadRequestErrorMessageResult).Message);
        }

        [Test]
        [Category("SensorEvents")]
        public void EvaluateRequestedDateTime_RequiresArgument_NotBeforeMaxLifetime()
        {
            IHttpActionResult badRequest = null;
            DateTime parsed;
            string argumentBeforeLifetime = _lifetimeStub.Since.AddHours(-1).ToString(SensorEventsController.ExpectedDateTimeFormat);

            bool success = _controller.EvaluateRequestedDateTime(argumentBeforeLifetime, out parsed, out badRequest);

            Assert.IsFalse(success);
            Assert.IsNotNull(badRequest as BadRequestErrorMessageResult);
            StringAssert.IsMatch("provided.+earlier than.+lifetime", (badRequest as BadRequestErrorMessageResult).Message);
        }

        [Test]
        [Category("SensorEvents")]
        public void EvaluateRequestedDateTime_WithValidDateTime_BadRequestIsNull_ReturnsTrue()
        {
            IHttpActionResult badRequest = new OkNegotiatedContentResult<object>(new object(), _controller);
            DateTime parsed = DateTime.MinValue;
            string validArgument = _lifetimeStub.Since.AddMinutes(1).ToString(SensorEventsController.ExpectedDateTimeFormat);

            bool success = _controller.EvaluateRequestedDateTime(validArgument, out parsed, out badRequest);

            Assert.IsTrue(success);
            Assert.IsNull(badRequest);
            Assert.AreEqual(_lifetimeStub.Since.AddMinutes(1), parsed);
        }

        [Test]
        [Category("SensorEvents")]
        public void AtMeterSince_GivenBadMeterId_ReturnsNotFound()
        {
            _mockMeteredSpacesService.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            string validDateArgument = _dateTimeStub.ToString(SensorEventsController.ExpectedDateTimeFormat);
            IHttpActionResult actionResult = _controller.AtMeterSince("no match", validDateArgument);
            NotFoundResult contentResult = actionResult as NotFoundResult;

            Assert.IsNotNull(contentResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void AtMeterSince_GivenGoodMeterIdAndDatetime_ReturnsMatchingSensorEventGETCollection()
        {
            anyMeterIdExists();

            DateTime sinceStub = _lifetimeStub.Since.AddMinutes(1);
            string meterIdStub = "match";

            _mockSensorEventsService
               .Setup(m => m.GetViewModelsSince(It.IsAny<DateTime>(), It.IsAny<string>()))
               .Returns<DateTime, string>((since, meterId) => new[] {
                    //MeterId matches, EventTime in range => return
                    new SensorEventGET() { MeterId = meterIdStub, EventTime = sinceStub },
                     //MeterId matches, EventTime in range => return
                    new SensorEventGET() { MeterId = meterIdStub, EventTime = sinceStub },
                    //MeterId matches, EventTime out of range => shouldn't return
                    new SensorEventGET() { MeterId = meterIdStub, EventTime = _lifetimeStub.Since.AddHours(-1) },
                    //MeterId doesn't match, EventTime in range => shouldn't return
                    new SensorEventGET() { MeterId = "don't match", EventTime = sinceStub },
                }.Where(vm => vm.MeterId == meterId && vm.EventTime >= since));

            IHttpActionResult actionResult = _controller.AtMeterSince(meterIdStub, sinceStub.ToString(SensorEventsController.ExpectedDateTimeFormat));
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            Assert.True(contentResult.Content.All(vm => vm.MeterId == meterIdStub));
            Assert.True(contentResult.Content.All(vm => vm.EventTime >= sinceStub));
        }

        [Test]
        [Category("SensorEvents")]
        public void AtMeter_GivenBadMeterId_ReturnsNotFound()
        {
            _mockMeteredSpacesService.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);

            IHttpActionResult actionResult = _controller.AtMeter("no match");
            NotFoundResult contentResult = actionResult as NotFoundResult;

            Assert.IsNotNull(contentResult);
        }

        [Test]
        [Category("SensorEvents")]
        public void AtMeter_WhenNoDefaultLifetime_UsesMaxLifetime()
        {
            anyMeterIdExists();

            _mockSensorEventsService
                .Setup(m => m.GetDefaultLifetime())
                .Returns(default(SensorEventLifetime));

            _mockSensorEventsService
                .Setup(m => m.GetMaxLifetime())
                .Returns(_lifetimeStub);

            _mockSensorEventsService
               .Setup(m => m.GetViewModelsSince(It.IsAny<DateTime>(), It.IsAny<string>()))
               .Returns<DateTime, string>((since, meterId) => new[] { new SensorEventGET() })
               .Callback<DateTime, string>((sinceArg, meterId) => Assert.AreEqual(_lifetimeStub.Since, sinceArg));

            IHttpActionResult actionResult = _controller.AtMeter("meterId");
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(1, contentResult.Content.Count());
        }

        [Test]
        [Category("SensorEvents")]
        public void AtMeter_GivenGoodMeterId_ReturnsMatchingSensorEventGETCollection_SinceDefaultLifetime()
        {
            anyMeterIdExists();

            _mockSensorEventsService.Setup(m => m.GetDefaultLifetime()).Returns(_lifetimeStub);

            string meterIdStub = "match";

            _mockSensorEventsService
               .Setup(m => m.GetViewModelsSince(It.IsAny<DateTime>(), It.IsAny<string>()))
               .Returns<DateTime, string>((since, meterId) => new[] {
                    //MeterId matches, EventTime in range => return
                    new SensorEventGET() { MeterId = meterIdStub, EventTime = _lifetimeStub.Since.AddMinutes(1) },
                    //MeterId matches, EventTime in range => return
                    new SensorEventGET() { MeterId = meterIdStub, EventTime = _lifetimeStub.Since.AddMinutes(5) },
                    //MeterId doesn't match => shouldn't return
                    new SensorEventGET() { MeterId = "nope", EventTime = _lifetimeStub.Since },
                    //EventTime out of range => shouldn't return
                    new SensorEventGET() { MeterId = meterIdStub, EventTime = _lifetimeStub.Since.AddHours(-1) },
                }.Where(vm => vm.MeterId == meterId && vm.EventTime >= since));

            IHttpActionResult actionResult = _controller.AtMeter(meterIdStub);
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            Assert.True(contentResult.Content.All(vm => vm.MeterId == meterIdStub));
            Assert.True(contentResult.Content.All(vm => vm.EventTime >= _lifetimeStub.Since));
        }

        [Test]
        [Category("SensorEvents")]
        public void Since_ReturnsMatchingSensorEventGETCollection()
        {
            anyMeterIdExists();

            DateTime sinceStub = _lifetimeStub.Since.AddMinutes(1);

            _mockSensorEventsService
               .Setup(m => m.GetViewModelsSince(It.IsAny<DateTime>()))
               .Returns<DateTime>((since) => new[] {
                    //EventTime in range => return
                    new SensorEventGET() { EventTime = sinceStub },
                    //EventTime in range => return
                    new SensorEventGET() { EventTime = sinceStub.AddMinutes(5) },
                    //EventTime out of range => shouldn't return
                    new SensorEventGET() { EventTime = sinceStub.AddMinutes(-1) },
                    //EventTime out of range => shouldn't return
                    new SensorEventGET() { EventTime = sinceStub.AddMinutes(-5) },
                }.Where(vm => vm.EventTime >= since));

            IHttpActionResult actionResult = _controller.Since(sinceStub.ToString(SensorEventsController.ExpectedDateTimeFormat));
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            Assert.True(contentResult.Content.All(vm => vm.EventTime >= sinceStub));
        }

        [Test]
        [Category("SensorEvents")]
        public void Default_WhenNoDefaultLifetime_UsesMaxLifetime()
        {
            anyMeterIdExists();

            _mockSensorEventsService
                .Setup(m => m.GetDefaultLifetime())
                .Returns(default(SensorEventLifetime));

            _mockSensorEventsService
                .Setup(m => m.GetMaxLifetime())
                .Returns(_lifetimeStub);

            _mockSensorEventsService
               .Setup(m => m.GetViewModelsSince(It.IsAny<DateTime>()))
               .Returns<DateTime>(since => new[] { new SensorEventGET() })
               .Callback<DateTime>(sinceArg => Assert.AreEqual(_lifetimeStub.Since, sinceArg));

            IHttpActionResult actionResult = _controller.Default();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(1, contentResult.Content.Count());
        }

        [Test]
        [Category("SensorEvents")]
        public void Default_ReturnsSensorEventGETCollection_SinceDefaultLifetime()
        {
            anyMeterIdExists();

            _mockSensorEventsService.Setup(m => m.GetDefaultLifetime()).Returns(_lifetimeStub);

            _mockSensorEventsService
               .Setup(m => m.GetViewModelsSince(It.IsAny<DateTime>()))
               .Returns<DateTime>((since) => new[] {
                    //EventTime in range => return
                    new SensorEventGET() { EventTime = _lifetimeStub.Since },
                    //EventTime in range => return
                    new SensorEventGET() { EventTime = _lifetimeStub.Since.AddMinutes(5) },
                    //EventTime out of range => shouldn't return
                    new SensorEventGET() { EventTime = _lifetimeStub.Since.AddMinutes(-1) },
                    //EventTime out of range => shouldn't return
                    new SensorEventGET() { EventTime = _lifetimeStub.Since.AddMinutes(-5) },
                }.Where(vm => vm.EventTime >= since));

            IHttpActionResult actionResult = _controller.Default();
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<SensorEventGET>>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.Count());
            Assert.True(contentResult.Content.All(vm => vm.EventTime >= _lifetimeStub.Since));
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
