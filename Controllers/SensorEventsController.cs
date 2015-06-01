using System;
using System.Globalization;
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
        private readonly IMeteredSpacesService _meteredSpacesService;
        private readonly ISensorEventsService _sensorEventsService;

        public ILogger Logger { get; set; }

        public SensorEventsController(
            IMeteredSpacesService meteredSpacesService,
            ISensorEventsService sensorEventsService)
        {
            _meteredSpacesService = meteredSpacesService;
            _sensorEventsService = sensorEventsService;

            Logger = NullLogger.Instance;
        }

        [TrackAnalytics("GET Sensor Events at Meter Since")]
        [HttpGet]
        public IHttpActionResult AtMeterSince(string meterId, string datetime)
        {
            if (!_meteredSpacesService.Exists(meterId))
                return NotFound();

            IHttpActionResult badRequest;
            DateTime datetimeParsed;
            if (!EvaluateRequestedDateTime(datetime, out datetimeParsed, out badRequest))
            {
                return badRequest;
            }

            var events = _sensorEventsService.GetViewModelsSince(datetimeParsed, meterId);
            return Ok(events);
        }

        [TrackAnalytics("GET Sensor Events at Meter")]
        [HttpGet]
        public IHttpActionResult AtMeter(string meterId)
        {
            if (!_meteredSpacesService.Exists(meterId))
                return NotFound();

            var lifetime = _sensorEventsService.GetDefaultLifetime();
            if (lifetime == null)
            {
                lifetime = _sensorEventsService.GetMaxLifetime();
            }

            var events = _sensorEventsService.GetViewModelsSince(lifetime.Since, meterId);
            return Ok(events);
        }

        [TrackAnalytics("GET Sensor Events Since")]
        [HttpGet]
        public IHttpActionResult Since(string datetime)
        {
            IHttpActionResult badRequest;
            DateTime datetimeParsed;

            if (!EvaluateRequestedDateTime(datetime, out datetimeParsed, out badRequest))
            {
                return badRequest;
            }

            var events = _sensorEventsService.GetViewModelsSince(datetimeParsed);
            return Ok(events);
        }

        [TrackAnalytics("GET Sensor Events")]
        [HttpGet]
        public IHttpActionResult Default()
        {
            var lifetime = _sensorEventsService.GetDefaultLifetime();
            if (lifetime == null)
            {
                lifetime = _sensorEventsService.GetMaxLifetime();
            }

            var events = _sensorEventsService.GetViewModelsSince(lifetime.Since);
            return Ok(events);
        }

        [RequireBasicAuthentication]
        [RequirePermissions("ApiWriter")]
        [ModelValidation]
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

            return Ok(_sensorEventsService.GetViewModel(entity));
        }

        internal static readonly string ExpectedDateTimeFormat = "yyyyMMddTHHmmssZ";

        internal bool EvaluateRequestedDateTime(string requestedDatetimeString, out DateTime parsed, out IHttpActionResult badRequest)
        {
            if (!DateTime.TryParseExact(requestedDatetimeString, SensorEventsController.ExpectedDateTimeFormat, null, DateTimeStyles.AdjustToUniversal, out parsed))
            {
                badRequest = BadRequest(String.Format("'{0}' could not be interpreted as an UTC ISO 8061 basic formatted DateTime.", requestedDatetimeString));
                return false;
            }

            if (parsed < _sensorEventsService.GetMaxLifetime().Since)
            {
                badRequest = BadRequest("The provided datetime was earlier than the maximum allowed lifetime.");
                return false;
            }

            badRequest = null;
            return true;
        }
    }
}
