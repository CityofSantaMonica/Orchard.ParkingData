using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;

namespace CSM.ParkingData.Tests
{
    public abstract class ControllerTestsBase : TestsBase
    {
        protected static readonly SensorEventLifetime _lifetimeStub = new SensorEventLifetime() { Length = 1.0, Units = LifetimeUnits.Hours, Since = _dateTimeStub };

        protected HttpRequestMessage _requestStub;
        protected HttpRequestContext _requestContextStub;
        protected Mock<ISensorEventsService> _mockSensorEventsService;
        protected Mock<IMeteredSpacesService> _mockMeteredSpacesService;

        public override void SetUp()
        {
            base.SetUp();

            _requestStub = new HttpRequestMessage() {
                Content = new StringContent("StringContent")
            };

            var mockRoute = new Mock<IHttpRoute>();
            mockRoute.SetupGet(m => m.RouteTemplate)
                     .Returns("RouteTemplate");

            var mockRouteData = new Mock<IHttpRouteData>();
            mockRouteData.SetupGet(m => m.Route)
                         .Returns(mockRoute.Object);

            _requestContextStub = new HttpRequestContext() {
                RouteData = mockRouteData.Object,
            };

            _mockSensorEventsService = new Mock<ISensorEventsService>();
            _mockSensorEventsService
                .Setup(m => m.GetViewModel(It.IsAny<SensorEvent>()))
                .Returns<SensorEvent>(
                    se => new SensorEventGET {
                        EventId = se.TransmissionId,
                        MeterId = (se.MeteredSpace ?? new MeteredSpace()).MeterId,
                        SessionId = se.SessionId,
                    }
                );
            _mockSensorEventsService
                .Setup(m => m.GetMaxLifetime())
                .Returns(_lifetimeStub);

            _mockMeteredSpacesService = new Mock<IMeteredSpacesService>();
            _mockMeteredSpacesService
                .Setup(m => m.ConvertToViewModel(It.IsAny<MeteredSpace>()))
                .Returns<MeteredSpace>(
                    ms => new MeteredSpaceGET {
                        Active = ms.Active,
                        MeterId = ms.MeterId
                    }
                );
        }

        protected IQueryable<SensorEvent> queryableSensorEvents()
        {
            return new[] {
                //EventTime in the "future" => should be included in the results
                new SensorEvent { TransmissionId = 1, EventTime = _dateTimeStub.AddHours(_lifetimeStub.Length * 1) },
                //EventTime in the "future" => should be included in the results
                new SensorEvent { TransmissionId = 2, EventTime = _dateTimeStub.AddHours(_lifetimeStub.Length * 2) },
                //EventTime in the "past" by more than lifetime => should be excluded
                new SensorEvent { TransmissionId = 3, EventTime = _dateTimeStub.AddHours(_lifetimeStub.Length * -4) }
            }.AsQueryable();
        }
    }
}
