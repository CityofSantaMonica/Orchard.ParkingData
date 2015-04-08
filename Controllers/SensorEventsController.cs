using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using CSM.ParkingData.Extensions;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using CSM.WebApi.Filters;
using GoogleAnalyticsTracker.WebApi2;
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

        public IHttpActionResult Get(long? id = null)
        {
            using (var tracker = new Tracker(analyticsId, _siteService.GetSiteSettings().BaseUrl))
            {
                tracker.TrackPageViewAsync(Request, "Sensor Events GET");
            }

            if (id.HasValue)
            {
                var theEvent = _sensorEventsService.Get(id.Value);

                if (theEvent == null)
                    return NotFound();
                else
                    return Ok(_sensorEventsService.ConvertToViewModel(theEvent));
            }
            else
            {
                var timeLimit = _clock.UtcNow.AddHours(-1 * _sensorEventsService.GetLifetimeHours());

                var events = _sensorEventsService.Query()
                                                 .Where(s => timeLimit <= s.EventTime)
                                                 .OrderByDescending(s => s.EventTime)
                                                 .Select(_sensorEventsService.ConvertToViewModel);
                return Ok(events);
            }
        }

        [RequireBasicAuthentication]
        [RequirePermissions("ApiWriter")]
        [ModelValidation]
        public IHttpActionResult Post([FromBody]SensorEventPOST postedSensorEvent)
        {
            using (var tracker = new Tracker(analyticsId, _siteService.GetSiteSettings().BaseUrl))
            {
                tracker.TrackPageViewAsync(Request, "Sensor Events POST");
            }

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
