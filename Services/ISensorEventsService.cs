using System;
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
        /// Get a value indicating the maximum time after a SensorEvent's EventTime that SensorEvent should be available on the public API.
        /// </summary>
        SensorEventLifetime GetMaxLifetime();

        /// <summary>
        /// Get a value indicating the time after a SensorEvent's EventTime that SensorEvent should be available on the default endpoint.
        /// </summary>
        SensorEventLifetime GetDefaultLifetime();

        /// <summary>
        /// Get a sensor event record by its transmission identifier.
        /// </summary>
        SensorEvent Get(long transmissionId);

        /// <summary>
        /// Get a queryable collection of sensor event records in the database.
        /// </summary>
        IQueryable<SensorEvent> Query();

        /// <summary>
        /// Get a queryable collection of sensor event records in the database, occuring since the specified DateTime.
        /// </summary>
        IQueryable<SensorEvent> QuerySince(DateTime since);

        /// <summary>
        /// Given the sensor event data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        SensorEvent AddOrUpdate(SensorEventPOST viewModel);

        /// <summary>
        /// Given a sensor event record, convert to an equivalent GET view model.
        /// </summary>
        SensorEventGET ConvertToViewModel(SensorEvent entity);
    }
}
