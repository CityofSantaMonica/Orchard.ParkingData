using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;
using NUnit.Framework;
using Orchard.Settings;

namespace CSM.ParkingData.Tests
{
    [TestFixture]
    public abstract class ControllerTestsBase
    {
        protected static readonly string _utcISO8061BasicFormat = "yyyyMMddTHHmmssZ";
        protected static readonly DateTime _referenceDateTime = new DateTime(2015, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        protected static readonly SensorEventLifetime _referenceMaxLifetime = new SensorEventLifetime() { Length = 1.0, Units = LifetimeUnits.Hours, Since = _referenceDateTime };

        protected HttpRequestMessage _mockRequest;
        protected HttpRequestContext _mockRequestContext;
        protected Mock<ISensorEventsService> _mockSensorEventsService;
        protected Mock<IMeteredSpacesService> _mockMeteredSpacesService;
        protected Mock<ISiteService> _mockSiteSevice;

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
                        EventId = se.TransmissionId,
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

            var mockSettings = new Mock<ISite>();
            mockSettings.Setup(m => m.BaseUrl).Returns("http://www.example.com");

            _mockSiteSevice = new Mock<ISiteService>();
            _mockSiteSevice
                .Setup(m => m.GetSiteSettings())
                .Returns(mockSettings.Object);
        }

        protected void setupMaxLifetime()
        {
            _mockSensorEventsService
                .Setup(m => m.GetMaxLifetime())
                .Returns(_referenceMaxLifetime);
        }

        protected void setupQuery()
        {
            setupMaxLifetime();

            var queryResult = new[] {
                //EventTime in the "future" => should be included in the results
                new SensorEvent { TransmissionId = 1, EventTime = _referenceDateTime.AddHours(_referenceMaxLifetime.Length * 1) },
                //EventTime in the "future" => should be included in the results
                new SensorEvent { TransmissionId = 2, EventTime = _referenceDateTime.AddHours(_referenceMaxLifetime.Length * 2) },
                //EventTime in the "past" by more than lifetime => should be excluded
                new SensorEvent { TransmissionId = 3, EventTime = _referenceDateTime.AddHours(_referenceMaxLifetime.Length * -4) }
            }.AsQueryable();

            _mockSensorEventsService
                .Setup(m => m.Query())
                .Returns(queryResult);

            _mockSensorEventsService
                .Setup(m => m.QuerySince(It.IsAny<DateTime>()))
                .Returns<DateTime>(since => queryResult.Where(se => se.EventTime >= since));

            _mockSensorEventsService
                .Setup(m => m.ConvertToViewModel(It.IsAny<SensorEvent>()))
                .Returns<SensorEvent>(s => new SensorEventGET() { EventId = s.TransmissionId, EventTime = s.EventTime });
        }
    }
}
