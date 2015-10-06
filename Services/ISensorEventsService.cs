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
        /// Return a queryable collection of SensorEvent records.
        /// </summary>
        IQueryable<SensorEvent> Query();

        /// <summary>
        /// Given the sensor event data in a POSTed view model, insert a new record or update an existing record.
        /// </summary>
        SensorEvent AddOrUpdate(SensorEventPOST viewModel);

        /// <summary>
        /// Convert the SensorEvent to an equivalent view model.
        /// </summary>
        SensorEventGET GetViewModel(SensorEvent entity);

        /// <summary>
        /// Gets the most recently recorded sensor event
        /// </summary>
        SensorEventGET GetLatestViewModel();

        /// <summary>
        /// Gets the most recently recorded sensor event at the MeteredSpace with the specified id.
        /// </summary>
        /// <param name="meterId"></param>
        SensorEventGET GetLatestViewModel(string meterId);

        /// <summary>
        /// Get a collection of view models occuring since the specified DateTime.
        /// </summary>
        IEnumerable<SensorEventGET> GetViewModelsSince(DateTime datetime);

        /// <summary>
        /// Get a collection of view models occuring since the specified DateTime at the MeteredSpace with the specified id.
        /// </summary>
        IEnumerable<SensorEventGET> GetViewModelsSince(DateTime datetime, string meterId);

        /// <summary>
        /// Get a collection of view models occuring since the specified ordinal number.
        /// </summary>
        IEnumerable<SensorEventGET> GetViewModelsSince(long ordinal);

        /// <summary>
        /// Get a collection of view models occuring since the specified ordinal number at the MeteredSpace with the specified id.
        /// </summary>
        IEnumerable<SensorEventGET> GetViewModelsSince(long ordinal, string meterId);
    }
}
