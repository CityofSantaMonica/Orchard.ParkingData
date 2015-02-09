using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;
using NUnit.Framework;
using MvcUrlHelper = System.Web.Mvc.UrlHelper;

namespace CSM.ParkingData.Tests
{
    [TestFixture]
    public abstract class ControllerTestsBase
    {
        protected HttpRequestMessage _mockRequest;
        protected HttpRequestContext _mockRequestContext;
        protected Mock<ISensorEventsService> _mockSensorEventsService;
        protected Mock<IMeteredSpacesService> _mockMeteredSpacesService;

        [SetUp]
        public virtual void TestsSetup()
        {
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

            _mockSensorEventsService = new Mock<ISensorEventsService>();
            _mockSensorEventsService
                .Setup(m => m.ConvertToViewModel(It.IsAny<SensorEvent>()))
                .Returns<SensorEvent>(
                    se => new SensorEventGET {
                        TransmissionId = se.TransmissionId,
                        MeterId = (se.MeteredSpace ?? new MeteredSpace()).MeterId,
                        SessionId = se.SessionId,
                    }
                );

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
    }
}
