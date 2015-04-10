using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using CSM.ParkingData.Extensions;
using CSM.ParkingData.Filters;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using CSM.WebApi.Filters;
using Orchard.Logging;

namespace CSM.ParkingData.Controllers
{
    [EnableCors("*", null, "GET")]
    public class SensorEventsController : ApiController
    {
        private readonly ISensorEventsService _sensorEventsService;

        public ILogger Logger { get; set; }

        public SensorEventsController(ISensorEventsService sensorEventsService)
        {
            _sensorEventsService = sensorEventsService;

            Logger = NullLogger.Instance;
        }

        [TrackAnalytics("GET Sensor Events Since")]
        [HttpGet]
        public IHttpActionResult Get(string datetime)
        {
            DateTime datetimeParsed;
            if (!DateTime.TryParseExact(datetime, "yyyyMMddTHHmmssZ", null, DateTimeStyles.AdjustToUniversal, out datetimeParsed))
            {
                return BadRequest(String.Format("'{0}' could not be interpreted as an UTC ISO 8061 basic formatted DateTime.", datetime));
            }

            var lifetime = _sensorEventsService.GetLifetime();

            if (datetimeParsed < lifetime.Since)
            {
                return BadRequest("The provided datetime was earlier than the allowed lifetime.");
            }

            var events = getSince(datetimeParsed);
            return Ok(events);
        }

        [TrackAnalytics("GET Sensor Events")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var lifetime = _sensorEventsService.GetLifetime();
            var events = getSince(lifetime.Since);
            return Ok(events);
        }

        [RequireBasicAuthentication]
        [RequirePermissions("ApiWriter")]
        [ModelValidation]
        [TrackAnalytics("POST Sensor Events")]
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

        private IEnumerable<SensorEventGET> getSince(DateTime datetime)
        {
            return _sensorEventsService.Query()
                                       .Where(s => datetime <= s.EventTime)
                                       .OrderByDescending(s => s.EventTime)
                                       .Select(_sensorEventsService.ConvertToViewModel);
        }
    }
}
