using System;
using System.Collections.Generic;
using System.Xml.Linq;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    /// <summary>
    /// Service definition for working with parking lots.
    /// </summary>
    public interface IParkingLotsService : IDependency
    {
        /// <summary>
        /// Get the collection of <see cref="CSM.ParkingData.ViewModels.ParkingLotGET"/> records.
        /// </summary>
        IEnumerable<ParkingLotGET> Get();

        /// <summary>
        /// Parse the xml representation of a parking lot into its <see cref="CSM.ParkingData.ViewModels.ParkingLotGET"/> representation.
        /// </summary>
        ParkingLotGET ParseFromXml(XElement xml);

        /// <summary>
        /// Parse the xml representation of the last update date and time into its <see cref="System.DateTime"/> representation.
        /// </summary>
        DateTime ParseLastUpdateUtc(XElement xml);
    }
}
