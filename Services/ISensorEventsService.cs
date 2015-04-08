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
        /// Get a sensor event record by its transmission identifier.
        /// </summary>
        SensorEvent Get(long transmissionId);

        /// <summary>
        /// Given the sensor event data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        SensorEvent AddOrUpdate(SensorEventPOST viewModel);

        /// <summary>
        /// Given a sensor event record, convert to an equivalent GET view model.
        /// </summary>
        SensorEventGET ConvertToViewModel(SensorEvent entity);

        /// <summary>
        /// Get a queryable collection of sensor event records in the database.
        /// </summary>
        IQueryable<SensorEvent> Query();

        /// <summary>
        /// Get a value indicating how long after a SensorEvent's EventTime that SensorEvent should be available on the public API.
        /// </summary>
        double GetLifetimeHours();
    }
}
