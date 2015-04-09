using System.Collections.Generic;
using System.Web.Http;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace CSM.ParkingData.Routes
{
    public class ApiRoutes : IHttpRouteProvider
    {
        private readonly string _area = "CSM.ParkingData";

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new HttpRouteDescriptor {
                    Name = "SensorEventsDefault",
                    Priority = 5,
                    RouteTemplate = "sensor_events",
                    Defaults = new {
                        area = _area,
                        controller = "SensorEvents"
                    }
                },
                new HttpRouteDescriptor() {
                    Name = "MeteredSpaces",
                    Priority = 5,
                    RouteTemplate = "metered_spaces/{id}",
                    Defaults = new {
                        area = _area,
                        controller = "MeteredSpaces",
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
