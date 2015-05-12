using System;
using System.Collections.Generic;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace CSM.ParkingData.Routes
{
    public class LifetimeRoutes : IHttpRouteProvider
    {
        private static readonly string _controller = "Lifetime";

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new HttpRouteDescriptor {
                    Name = "SensorEventsMaxLifetime",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = String.Format("{0}/lifetime/max", Routes.BaseEventRoute),
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "Max"
                    }
                },
                new HttpRouteDescriptor {
                    Name = "SensorEventsDefaultLifetime",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = String.Format("{0}/lifetime", Routes.BaseEventRoute),
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "Default"
                    }
                },
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
