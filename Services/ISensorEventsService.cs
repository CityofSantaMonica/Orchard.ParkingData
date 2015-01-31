using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    /// <summary>
    /// Service definition for working with sensor events.
    /// </summary>
    public interface ISensorEventsService : IDependency
    {
        /// <summary>
        /// Given the sensor event data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        /// <param name="viewModel">A valid sensor event POST view model.</param>
        /// <returns>The inserted/updated sensor event record.</returns>
        SensorEvent AddOrUpdate(SensorEventPOST viewModel);

        /// <summary>
        /// Given a sensor event record, convert to an equivalent GET view model.
        /// </summary>
        /// <param name="entity">An existing sensor event record.</param>
        /// <returns>The GET view model representation of <paramref name="entity"/>.</returns>
        SensorEventGET ConvertToViewModel(SensorEvent entity);

        /// <summary>
        /// Get a queryable collection of sensor event records in the database.
        /// </summary>
        IQueryable<SensorEvent> QueryEntities();

        /// <summary>
        /// Get a queryable collection of sensor event records in the database, represented as GET view models.
        /// </summary>
        IQueryable<SensorEventGET> QueryViewModels();
    }
}
