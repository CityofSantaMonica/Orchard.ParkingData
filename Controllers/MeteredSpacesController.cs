using System;
using System.Collections.Generic;
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
    public class MeteredSpacesController : ApiController
    {
        private readonly IMeteredSpacesService _meteredSpacesService;

        public ILogger Logger { get; set; }

        public MeteredSpacesController(IMeteredSpacesService meteredSpacesService)
        {
            _meteredSpacesService = meteredSpacesService;
            Logger = NullLogger.Instance;
        }

        public IHttpActionResult Get()
        {
            var spaces = _meteredSpacesService.QueryViewModels()
                                              .ToList();
            return Ok(spaces);
        }

        [RequireBasicAuthentication]
        [RequirePermissions("ApiWriter")]
        public IHttpActionResult Post([FromBody]IEnumerable<MeteredSpacePOST> postedMeteredSpaces)
        {
            if (postedMeteredSpaces == null || !postedMeteredSpaces.Any() || !ModelState.IsValid)
            {
                Logger.Warning("POST with invalid model{0}{1}", Environment.NewLine, Request.Content.ReadAsStringAsync().Result);
                return BadRequest();
            }

            try
            {
                foreach (var postedSpace in postedMeteredSpaces)
                {
                    _meteredSpacesService.AddOrUpdate(postedSpace);
                }
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
