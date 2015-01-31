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
        /// Given the metered space data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        MeteredSpace AddOrUpdate(MeteredSpacePOST viewModel);

        /// <summary>
        /// Given the metered space data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        MeteredSpace AddOrUpdate(SensorEventMeteredSpacePOST viewModel);

        /// <summary>
        /// Given a metered space record, convert to an equivalent GET view model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        MeteredSpaceGET ConvertToViewModel(MeteredSpace entity);

        /// <summary>
        /// Get a queryable collection of metered space records in the database.
        /// </summary>
        /// <returns></returns>
        IQueryable<MeteredSpace> QueryEntities();

        /// <summary>
        /// Get a queryable collection of metered space records in the database, represented as GET view models.
        /// </summary>
        /// <returns></returns>
        IQueryable<MeteredSpaceGET> QueryViewModels();
    }
}
