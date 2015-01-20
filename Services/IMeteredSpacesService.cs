using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    public interface IMeteredSpacesService : IDependency
    {
        MeteredSpace ConvertToEntity(SensorEventMeteredSpacePOST viewModel);
        MeteredSpace ConvertToEntity(MeteredSpacePOST viewModel);
        IQueryable<MeteredSpace> QueryEntities();
        IQueryable<MeteredSpaceGET> QueryViewModels();
        bool TryAddSpace(MeteredSpace entity);
    }
}
