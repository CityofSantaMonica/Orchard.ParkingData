using System.Collections.Generic;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    public interface IParkingLotsService : IDependency
    {
        IEnumerable<ParkingLot> Get();
    }
}
