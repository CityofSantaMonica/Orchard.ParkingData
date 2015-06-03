using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using CSM.ParkingData.Filters;
using CSM.ParkingData.Services;

namespace CSM.ParkingData.Controllers
{
    [EnableCors("*", null, "GET")]
    public class LotsController : ApiController
    {
        private readonly IParkingLotsService _parkingLotsService;

        public LotsController(IParkingLotsService parkingLotsService)
        {
            _parkingLotsService = parkingLotsService;
        }

        [TrackAnalytics("GET Lots")]
        [HttpGet]
        public IHttpActionResult Get(string name)
        {
            var lots = _parkingLotsService.Get();

            if(!String.IsNullOrEmpty(name))
                lots = lots.Where(l => l.Name.ToLower().Contains(name.ToLower()));

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
