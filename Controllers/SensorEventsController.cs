using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using CSM.ParkingData.Extensions;
using CSM.ParkingData.Filters;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using CSM.WebApi.Filters;
using Microsoft.WindowsAzure;
using Orchard.Logging;
using Orchard.Services;
using Orchard.Settings;

namespace CSM.ParkingData.Controllers
{
    [EnableCors("*", null, "GET")]
    public class SensorEventsController : ApiController
    {
        static string analyticsId = CloudConfigurationManager.GetSetting("GoogleAnalyticsId");

        private readonly IClock _clock;
        private readonly ISensorEventsService _sensorEventsService;
        private readonly ISiteService _siteService;

        public ILogger Logger { get; set; }

        public SensorEventsController(
            IClock clock,
            ISensorEventsService sensorEventsService,
            ISiteService siteService)
        {
            _clock = clock;
            _sensorEventsService = sensorEventsService;
            _siteService = siteService;

            Logger = NullLogger.Instance;
        }

        [TrackAnalytics("Sensor Events GET")]
        [HttpGet]
        public IHttpActionResult GetDefault()
        {
            var lifetime = _sensorEventsService.GetLifetime();

            var events = _sensorEventsService.Query()
                                             .Where(s => lifetime.Since <= s.EventTime)
                                             .OrderByDescending(s => s.EventTime)
                                             .Select(_sensorEventsService.ConvertToViewModel);
            return Ok(events);
        }

        [TrackAnalytics("Sensor Events Lifetime GET")]
        [HttpGet]
        public IHttpActionResult GetLifetime()
        {
            var lifetime = _sensorEventsService.GetLifetime();
            return Ok(lifetime);
        }

        [RequireBasicAuthentication]
        [RequirePermissions("ApiWriter")]
        [ModelValidation]
        [TrackAnalytics("Sensor Events POST")]
        public IHttpActionResult Post([FromBody]SensorEventPOST postedSensorEvent)
        {
            if (postedSensorEvent == null)
            {
                Logger.Warning("POST to {0} with null model.", RequestContext.RouteData.Route.RouteTemplate);
                return BadRequest("Incoming data parsed to null entity model.");
            }

            SensorEvent entity = null;

            try
            {
                entity = _sensorEventsService.AddOrUpdate(postedSensorEvent);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    ex,
                    "Server error on POST to {0} with model:{1}{2}",
                    RequestContext.RouteData.Route.RouteTemplate,
                    Environment.NewLine,
                    postedSensorEvent.ToXmlString()
                );
                return InternalServerError(ex);
            }

            //temporary because WebApi routes are registered with Route.Name = null, hence cannot be looked up by name
            //we should return CreatedAtRoute (201 with a location header)
            //instead we just return 200 with the entity

            //return CreatedAtRoute(
            //    "SensorEvents",
            //    new { id = entity.Id },
            //    entity
            //);

            return Ok(_sensorEventsService.ConvertToViewModel(entity));
        }
    }
}
