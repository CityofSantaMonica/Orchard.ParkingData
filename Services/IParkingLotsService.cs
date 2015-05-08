using CSM.ParkingData.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CSM.ParkingData.Services
{
    public interface IParkingLotsService : IDependency
    {
        IEnumerable<ParkingLot> Get();
        ParkingLot ParseFromXml(XElement xml);
        DateTime ParseLastUpdateUtc(XElement xml);
    }
}
