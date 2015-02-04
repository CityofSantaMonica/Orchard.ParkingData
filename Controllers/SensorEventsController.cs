using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
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

        [CompressResponse]
        public IHttpActionResult Get(long? id = null)
        {
            if (id.HasValue)
            {
                var theEvent = _sensorEventsService.Query()
                                                   .Where(s => s.TransmissionId == id.Value)
                                                   .SingleOrDefault();
                if (theEvent == null)
                    return NotFound();
                else
                    return Ok(_sensorEventsService.ConvertToViewModel(theEvent));
            }
            else
            {
                var events = _sensorEventsService.Query()
                                                 .OrderByDescending(s => s.EventTime)
                                                 .Take(1000)
                                                 .Select(_sensorEventsService.ConvertToViewModel);
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
                    String.Format(
                        "Server error on POST to {0} with model: {1}",
                        RequestContext.RouteData.Route.RouteTemplate,
                        Request.Content.ReadAsStringAsync().Result
                    )
                );
                return InternalServerError(ex);
            }

            return CreatedAtRoute(
                "SensorEvents",
                new { id = entity.TransmissionId },
                _sensorEventsService.ConvertToViewModel(entity)
            );
        }
    }
}
