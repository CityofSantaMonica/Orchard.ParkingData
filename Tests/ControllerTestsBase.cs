using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Moq;
using NUnit.Framework;

namespace CSM.ParkingData.Tests
{
    [TestFixture]
    public abstract class ControllerTestsBase
    {
        protected HttpRequestMessage _mockRequest;
        protected HttpRequestContext _mockRequestContext;

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
        }
    }
}
