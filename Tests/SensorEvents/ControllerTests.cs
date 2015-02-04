using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
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
        private readonly Mock<ISensorEventsService> _mockSensorEventService;

        public ControllerTests()
        {
            _mockSensorEventService = new Mock<ISensorEventsService>();

            _mockSensorEventService
                .Setup(m => m.ConvertToViewModel(It.IsAny<SensorEvent>()))
                .Returns<SensorEvent>(
                    se => new SensorEventGET {
                        TransmissionId = se.TransmissionId,
                        SessionId = se.SessionId
                    }
                );
        }

        [Test]
        [Category("SensorEvents")]
        public void Get_Returns_SensorEventGETCollection()
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
        public void Get_Returns_SensorEventGET()
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
        public void Get_Returns_NotFound()
        {
            var controller = new SensorEventsController(_mockSensorEventService.Object);

            IHttpActionResult actionResult = controller.Get(-100);

            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }
    }
}
