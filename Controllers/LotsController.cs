using System.Linq;
using System.Web.Http;
using CSM.ParkingData.Filters;
using CSM.ParkingData.Services;
using CSM.WebApi.Filters;

namespace CSM.ParkingData.Controllers
{
    [EnableGlobalCors("GET")]
    public class LotsController : ApiController
    {
        private readonly IParkingLotsService _parkingLotsService;

        public LotsController(IParkingLotsService parkingLotsService)
        {
            _parkingLotsService = parkingLotsService;
        }

        [TrackAnalytics("GET Lots")]
        [HttpGet]
        public IHttpActionResult Get(long? id = null)
        {
            var lots = _parkingLotsService.Get();

            if(id.HasValue)
                lots = lots.Where(l => l.Id == id);

            if(lots.Any())
            {
                return Ok(lots);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
