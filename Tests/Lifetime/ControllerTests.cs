using System;
using System.Web.Http;
using System.Web.Http.Results;
using CSM.ParkingData.Controllers;
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
        [Category("SensorEvents")]
        public void GetLifetime_ReturnsSensorEventLifetime()
        {
            base.setupLifetime();

            IHttpActionResult actionResult = _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<SensorEventLifetime>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(_referenceLifetime.Length, contentResult.Content.Length);
            Assert.AreEqual(DateTimeKind.Utc, contentResult.Content.Since.Kind);
            Assert.AreEqual(_referenceLifetime.Since, contentResult.Content.Since);
            Assert.AreEqual(_referenceLifetime.Units, contentResult.Content.Units);
        }
    }
}
