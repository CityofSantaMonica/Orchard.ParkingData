using System.Web.Http;
using CSM.ParkingData.Filters;
using CSM.ParkingData.Services;
using CSM.WebApi.Filters;

namespace CSM.ParkingData.Controllers
{
    [EnableGlobalCors("GET")]
    public class LifetimeController : ApiController
    {
        private readonly ISensorEventsService _sensorEventsService;

        public LifetimeController(ISensorEventsService sensorEventsService)
        {
            _sensorEventsService = sensorEventsService;
        }

        [TrackAnalytics("GET Max Sensor Events Lifetime")]
        [HttpGet]
        public IHttpActionResult Max()
        {
            var lifetime = _sensorEventsService.GetMaxLifetime();
            return Ok(lifetime);
        }

        [TrackAnalytics("GET Sensor Events Lifetime")]
        [HttpGet]
        public IHttpActionResult Default()
        {
            var lifetime = _sensorEventsService.GetDefaultLifetime();

            if (lifetime == null)
                lifetime = _sensorEventsService.GetMaxLifetime();

            return Ok(lifetime);
        }
    }
}
