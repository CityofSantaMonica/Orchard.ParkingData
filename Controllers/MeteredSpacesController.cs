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
    public class MeteredSpacesController : ApiController
    {
        private readonly IMeteredSpacesService _meteredSpacesService;

        public ILogger Logger { get; set; }

        public MeteredSpacesController(IMeteredSpacesService meteredSpacesService)
        {
            _meteredSpacesService = meteredSpacesService;

            Logger = NullLogger.Instance;
        }

        public IHttpActionResult Get(string id = null)
        {
            if (String.IsNullOrEmpty(id))
            {
                var spaces =
                    _meteredSpacesService.Query()
                                         .Select(_meteredSpacesService.ConvertToViewModel)
                                         .ToArray();
                return Ok(spaces);
            }
            else
            {
                var space = _meteredSpacesService.Get(id);

                if (space == null)
                    return NotFound();
                else
                    return Ok(_meteredSpacesService.ConvertToViewModel(space));
            }
        }

        //[RequireBasicAuthentication]
        //[RequirePermissions("ApiWriter")]
        [ModelValidation]
        public IHttpActionResult Post([FromBody]MeteredSpacePOSTCollection postedMeteredSpaces)
        {
            if (postedMeteredSpaces == null)
            {
                Logger.Warning("POST to {0} with null model", RequestContext.RouteData.Route.RouteTemplate);
                return BadRequest("Incoming data parsed to null entity model.");
            }

            if (postedMeteredSpaces.Count == 0)
            {
                Logger.Warning("POST to {0} with empty model", RequestContext.RouteData.Route.RouteTemplate);
                return BadRequest("Incoming data parsed to empty entity model.");
            }

            MeteredSpace lastEntity = null;

            try
            {
                foreach (var postedSpace in postedMeteredSpaces)
                {
                    lastEntity = _meteredSpacesService.AddOrUpdate(postedSpace);
                }
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

            //temporary because WebApi routes are registered with Route.Name = null, hence cannot be looked up by name
            //we should return CreatedAtRoute (201 with a location header)
            //instead we just return 200 with the entity

            //return CreatedAtRoute(
            //    "MeteredSpaces", 
            //    new { id = lastEntity.MeterId },
            //    _meteredSpacesService.ConvertToViewModel(lastEntity)
            //);

            return Ok(_meteredSpacesService.ConvertToViewModel(lastEntity));
        }
    }
}
