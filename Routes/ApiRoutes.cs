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
                    Name = "SensorEventsSince",
                    Priority = 6,
                    RouteTemplate = "sensor_events/since/{datetime}",
                    Defaults = new {
                        area = _area,
                        controller = "SensorEvents"
                    },
                    Constraints = new {
                        datetime = @"\d{8}T\d{6}Z"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "SensorEventsMaxLifetime",
                    Priority = 6,
                    RouteTemplate = "sensor_events/lifetime/max",
                    Defaults = new {
                        area = _area,
                        controller = "Lifetime",
                        action = "Max"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "SensorEventsDefaultLifetime",
                    Priority = 6,
                    RouteTemplate = "sensor_events/lifetime",
                    Defaults = new {
                        area = _area,
                        controller = "Lifetime",
                        action = "Default"
                    }
                },
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
                },
                new HttpRouteDescriptor() {
                    Name = "Lots",
                    Priority = 5,
                    RouteTemplate = "lots",
                    Defaults = new {
                        area = _area,
                        controller = "Lots"
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
