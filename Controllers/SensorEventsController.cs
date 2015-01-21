using System;
using System.Linq;
using System.Web.Http;
using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using CSM.Security.Filters;
using Orchard.Logging;

namespace CSM.ParkingData.Controllers
{
    [EnableCors(origins: "*", methods: "GET")]
    public class SensorEventsController : ApiController
    {
        private readonly ISensorEventsService _sensorEventsService;

        public SensorEventsController(ISensorEventsService sensorEventsService)
        {
            _sensorEventsService = sensorEventsService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IHttpActionResult Get(int limit = int.MaxValue)
        {
            var events = _sensorEventsService.QueryViewModels()
                                             .Take(limit)
                                             .ToList();
            return Ok(events);
        }

        [RequireBasicAuthentication("ApiWriter")]
        public IHttpActionResult Post([FromBody]SensorEventPOST sensorEvent)
        {
            if (sensorEvent == null || !ModelState.IsValid)
            {
                Logger.Warning("POST with invalid model{0}{1}", Environment.NewLine, Request.Content.ReadAsStringAsync().Result);
                return BadRequest();
            }

            SensorEvent entity = null;

            try
            {
                entity = _sensorEventsService.ConvertToEntity(sensorEvent);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, String.Format("POST with invalid model{0}{1}", Environment.NewLine, Request.Content.ReadAsStringAsync().Result));
                return BadRequest();
            }

            if (_sensorEventsService.TryAddEvent(entity))
            {
                return Ok();
            }
            else
            {
                Logger.Error(String.Format("Server Error saving POSTed model{0}{1}", Environment.NewLine, Request.Content.ReadAsStringAsync().Result));
                return InternalServerError();
            }
        }
    }
}
