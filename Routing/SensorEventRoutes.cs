using System.Collections.Generic;
using System.Web.Http;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace CSM.ParkingData.Routing
{
    public class SensorEventRoutes : IHttpRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new HttpRouteDescriptor {
                    Priority = 5,
                    Name = "v0 Sensor Events",
                    RouteTemplate = "v0/sensor_events/{limit}",
                    Defaults = new {
                        area = "CSM.ParkingData",
                        controller = "SensorEvents",
                        limit = RouteParameter.Optional
                    }
                }
            };
        }
    }
}