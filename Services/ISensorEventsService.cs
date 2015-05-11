using System;
using System.Collections.Generic;
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
        /// Get a queryable collection of sensor event records.
        /// </summary>
        IQueryable<SensorEvent> Query();

        /// <summary>
        /// Get a queryable collection of sensor event records occuring at the MeteredSpace with the specified id.
        /// </summary>
        IQueryable<SensorEvent> Query(string meterId);

        /// <summary>
        /// Get a queryable collection of sensor event records occuring since the specified DateTime.
        /// </summary>
        IQueryable<SensorEvent> QuerySince(DateTime since);

        /// <summary>
        /// Get a queryable collection of sensor event records occuring since the specified DateTime at the MeteredSpace with the specified id.
        /// </summary>
        IQueryable<SensorEvent> QuerySince(DateTime since, string meterId);

        /// <summary>
        /// Given the sensor event data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        SensorEvent AddOrUpdate(SensorEventPOST viewModel);

        /// <summary>
        /// Given a sensor event record, convert to an equivalent GET view model.
        /// </summary>
        SensorEventGET GetViewModel(SensorEvent entity);

        /// <summary>
        /// Get a collection of sensor event GET view models occuring since the specified DateTime.
        /// </summary>
        IEnumerable<SensorEventGET> GetViewModelsSince(DateTime since);

        /// <summary>
        /// Get a collection of sensor event GET view models occuring since the specified DateTime at the MeteredSpace with the specified id.
        /// </summary>
        IEnumerable<SensorEventGET> GetViewModelsSince(DateTime since, string meterId);
    }
}
