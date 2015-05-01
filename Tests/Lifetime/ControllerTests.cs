using System;
using System.Web.Http;
using System.Web.Http.Results;
using CSM.ParkingData.Controllers;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.Lifetime
{
    public class ControllerTests : ControllerTestsBase
    {
        private LifetimeController _controller;

        [SetUp]
        public override void TestsSetup()
        {
            base.TestsSetup();

            _controller = new LifetimeController(_mockSensorEventsService.Object) {
                Request = _mockRequest,
                RequestContext = _mockRequestContext
            };
        }

        [Test]
        [Category("SensorEventsLifetime")]
        public void GetMax_ReturnsSensorEventMaxLifetime()
        {
            base.setupMaxLifetime();

            IHttpActionResult actionResult = _controller.Max();
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventLifetime>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(_referenceMaxLifetime.Length, contentResult.Content.Length);
            Assert.AreEqual(DateTimeKind.Utc, contentResult.Content.Since.Value.Kind);
            Assert.AreEqual(_referenceMaxLifetime.Since, contentResult.Content.Since);
            Assert.AreEqual(_referenceMaxLifetime.Units, contentResult.Content.Units);
        }

        [Test]
        [Category("SensorEventsLifetime")]
        public void GetDefault_ReturnsSensorEventMaxLifetime_WhenNoDefaultConfigured()
        {
            base.setupMaxLifetime();

            SensorEventLifetime notConfigured = null;

            _mockSensorEventsService
                .Setup(m => m.GetDefaultLifetime())
                .Returns(notConfigured);

            IHttpActionResult actionResult = _controller.Default();
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventLifetime>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(_referenceMaxLifetime.Length, contentResult.Content.Length);
            Assert.AreEqual(DateTimeKind.Utc, contentResult.Content.Since.Value.Kind);
            Assert.AreEqual(_referenceMaxLifetime.Since, contentResult.Content.Since);
            Assert.AreEqual(_referenceMaxLifetime.Units, contentResult.Content.Units);
        }

        [Test]
        [Category("SensorEventsLifetime")]
        public void GetDefault_ReturnsSensorEventDefaultLifetime()
        {
            var mockDefaultLifetime = new SensorEventLifetime {
                Length = 1,
                Units = LifetimeUnits.Hours,
                Since = new DateTime(1, DateTimeKind.Utc)
            };

            _mockSensorEventsService
                .Setup(m => m.GetDefaultLifetime())
                .Returns(mockDefaultLifetime);

            IHttpActionResult actionResult = _controller.Default();
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventLifetime>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(mockDefaultLifetime.Length, contentResult.Content.Length);
            Assert.AreEqual(DateTimeKind.Utc, contentResult.Content.Since.Value.Kind);
            Assert.AreEqual(mockDefaultLifetime.Since, contentResult.Content.Since);
            Assert.AreEqual(mockDefaultLifetime.Units, contentResult.Content.Units);
        }
    }
}
