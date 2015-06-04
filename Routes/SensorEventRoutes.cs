using System;
using System.Collections.Generic;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace CSM.ParkingData.Routes
{
    public class SensorEventRoutes : IHttpRouteProvider
    {
        private static readonly string _controller = "SensorEvents";

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new HttpRouteDescriptor() {
                    Name = "SensorEventsAtMeterSinceDateTime",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = String.Format("{0}/since/{{datetime}}", Routes.BaseEventAtRoute),
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "AtMeterSinceDateTime"
                    },
                    Constraints = new {
                        datetime = Routes.DateTimeConstraint
                    }
                },
                new HttpRouteDescriptor() {
                    Name = "SensorEventsAtMeterSinceSequence",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = String.Format("{0}/since/{{sequence}}", Routes.BaseEventAtRoute),
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "AtMeterSinceSequence"
                    },
                    Constraints = new {
                        sequence = Routes.SequenceConstraint
                    }
                },
                new HttpRouteDescriptor() {
                    Name = "SensorEventsAtMeter",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = Routes.BaseEventAtRoute,
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "AtMeter"
                    }
                },
                new HttpRouteDescriptor() {
                    Name = "SensorEventsSinceDateTime",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = String.Format("{0}/since/{{datetime}}", Routes.BaseEventRoute),
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "SinceDateTime"
                    },
                    Constraints = new {
                        datetime = Routes.DateTimeConstraint
                    }
                },
                 new HttpRouteDescriptor() {
                    Name = "SensorEventsSinceSequence",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = String.Format("{0}/since/{{sequence}}", Routes.BaseEventRoute),
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "SinceSequence"
                    },
                    Constraints = new {
                        sequence = Routes.SequenceConstraint
                    }
                },
                new HttpRouteDescriptor() {
                    Name = "SensorEventsDefault",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = Routes.BaseEventRoute,
                    Defaults = new {
                        area = Routes.Area,
                        controller = _controller,
                        action = "Default"
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
