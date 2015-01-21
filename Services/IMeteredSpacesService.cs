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
        MeteredSpaceGET ConvertToViewModel(MeteredSpace entity);
        IQueryable<MeteredSpace> QueryEntities();
        IQueryable<MeteredSpaceGET> QueryViewModels();
        bool TryAddSpace(MeteredSpacePOST viewModel);
        bool TryAddSpace(MeteredSpace entity);
    }
}
