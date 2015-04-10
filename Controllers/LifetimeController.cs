using System.Web.Http;
using System.Web.Http.Cors;
using CSM.ParkingData.Filters;
using CSM.ParkingData.Services;

namespace CSM.ParkingData.Controllers
{
    [EnableCors("*", null, "GET")]
    public class LifetimeController : ApiController
    {
        private readonly ISensorEventsService _sensorEventsService;

        public LifetimeController(ISensorEventsService sensorEventsService)
        {
            _sensorEventsService = sensorEventsService;
        }

        [TrackAnalytics("GET Sensor Events Lifetime")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var lifetime = _sensorEventsService.GetLifetime();
            return Ok(lifetime);
        }
    }
}
