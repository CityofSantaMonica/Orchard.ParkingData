using System.Collections.Generic;
using System.Web.Http;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace CSM.ParkingData.Routing
{
    public class SensorEventRoutes : IHttpRouteProvider
    {
        private readonly string _area = "CSM.ParkingData";

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new HttpRouteDescriptor {
                    Priority = 5,
                    Name = "Sensor Events",
                    RouteTemplate = "sensor_events/{id}",
                    Defaults = new {
                        area = _area,
                        controller = "SensorEvents",
                        id = RouteParameter.Optional
                    }
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }
    }
}