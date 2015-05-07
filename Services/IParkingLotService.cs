using System.Collections.Generic;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    public interface IParkingLotService : IDependency
    {
        IEnumerable<ParkingLot> Get();
    }
}
