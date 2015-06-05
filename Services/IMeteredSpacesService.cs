using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    /// <summary>
    /// Service definition for working with metered spaces.
    /// </summary>
    public interface IMeteredSpacesService : IDependency
    {
        /// <summary>
        /// Get the configured cache settings for metered spaces.
        /// </summary>
        /// <returns></returns>
        MeteredSpaceCacheSettings GetCacheSettings();

        /// <summary>
        /// Get a metered space record by its meter identifier.
        /// </summary>
        MeteredSpace Get(string meterId);

        /// <summary>
        /// Get a flag indicating if the meter with the specified id exists.
        /// </summary>
        bool Exists(string meterId);

        /// <summary>
        /// Given the metered space data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        MeteredSpace AddOrUpdate(MeteredSpacePOST viewModel);

        /// <summary>
        /// Given the metered space data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        MeteredSpace AddOrUpdate(SensorEventMeteredSpacePOST viewModel);

        /// <summary>
        /// Given a metered space record, convert to an equivalent GET view model.
        /// </summary>
        MeteredSpaceGET ConvertToViewModel(MeteredSpace entity);

        /// <summary>
        /// Get a queryable collection of metered space records.
        /// </summary>
        IQueryable<MeteredSpace> Query();
    }
}
