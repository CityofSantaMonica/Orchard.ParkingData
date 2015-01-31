using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
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

        public IHttpActionResult Get(int limit = 1000)
        {
            var events = _sensorEventsService.QueryViewModels()
                                             .OrderByDescending(s => s.EventTime)
                                             .Take(limit);
            return Ok(events);
        }

        [RequireBasicAuthentication]
        [RequirePermissions("ApiWriter")]
        public IHttpActionResult Post([FromBody]SensorEventPOST postedSensorEvent)
        {
            if (postedSensorEvent == null || !ModelState.IsValid)
            {
                Logger.Warning("POST with invalid model{0}{1}", Environment.NewLine, Request.Content.ReadAsStringAsync().Result);
                return BadRequest();
            }

            try
            {
                _sensorEventsService.AddOrUpdate(postedSensorEvent);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, String.Format("Server error saving POSTed model{0}{1}", Environment.NewLine, Request.Content.ReadAsStringAsync().Result));
                return InternalServerError();
            }

            return Ok();
        }
    }
}
