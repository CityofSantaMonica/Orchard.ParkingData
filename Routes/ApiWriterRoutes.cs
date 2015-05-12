using System.Collections.Generic;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace CSM.ParkingData.Routes
{
    public class ApiWriterRoutes : IHttpRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new HttpRouteDescriptor {
                    Name = "SensorEventsWriter",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = "sensor_events",
                    Defaults = new {
                        area = Routes.Area,
                        controller = "SensorEvents"
                    }
                },
                new HttpRouteDescriptor() {
                    Name = "MeteredSpacesWriter",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = "metered_spaces",
                    Defaults = new {
                        area = Routes.Area,
                        controller = "MeteredSpaces"
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
