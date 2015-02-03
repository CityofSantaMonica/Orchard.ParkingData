using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using CSM.Security.Filters.Http;
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

        public IHttpActionResult Get(long? id = null)
        {
            if (id.HasValue)
            {
                var theEvent = _sensorEventsService.QueryViewModels()
                                                   .Where(s => s.TransmissionId == id.Value)
                                                   .SingleOrDefault();
                if (theEvent == null)
                    return NotFound();
                else
                    return Ok(theEvent);
            }
            else
            {
                var events = _sensorEventsService.QueryViewModels()
                                                 .OrderByDescending(s => s.EventTime)
                                                 .Take(1000);
                return Ok(events);
            }
        }

        [RequireBasicAuthentication]
        [RequirePermissions("ApiWriter")]
        [ModelValidation]
        public IHttpActionResult Post([FromBody]SensorEventPOST postedSensorEvent)
        {
            if (postedSensorEvent == null)
            {
                Logger.Warning("POST to /sensor_events with null model.");
                return BadRequest("Incoming data parsed to null entity model.");
            }

            SensorEvent entity = null;

            try
            {
                entity = _sensorEventsService.AddOrUpdate(postedSensorEvent);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, String.Format("Server error on POST to /sensor_events with model:{0}{1}", Environment.NewLine, Request.Content.ReadAsStringAsync().Result));
                return InternalServerError(ex);
            }

            return Created("", entity);
        }
    }
}
