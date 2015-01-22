using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    public interface ISensorEventsService : IDependency
    {
        SensorEvent AddOrUpdate(SensorEventPOST viewModel);
        SensorEventGET ConvertToViewModel(SensorEvent entity);
        IQueryable<SensorEvent> QueryEntities();
        IQueryable<SensorEventGET> QueryViewModels();
    }
}
