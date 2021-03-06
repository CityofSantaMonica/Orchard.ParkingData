﻿using System.Collections.Generic;
using System.Web.Http;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace CSM.ParkingData.Routes
{
    public class LotRoutes : IHttpRouteProvider
    {
        private static readonly string _controller = "Lots";

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new HttpRouteDescriptor() {
                    Name = "LotsMatching",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = "lots/matching/{name}",
                    Defaults = new {
                        area = Routes.Area,
                        action = "GetMatching",
                        controller = _controller,
                    },
                    Constraints = new { 
                        name = Routes.NonEmptyStringContraint
                    }
                },
                new HttpRouteDescriptor() {
                    Name = "LotsDefault",
                    Priority = Routes.DefaultPriority,
                    RouteTemplate = "lots/{id}",
                    Defaults = new {
                        area = Routes.Area,
                        action = "GetDefault",
                        controller = _controller,
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
