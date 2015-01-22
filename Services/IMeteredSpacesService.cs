using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard;

namespace CSM.ParkingData.Services
{
    public interface IMeteredSpacesService : IDependency
    {
        MeteredSpace AddOrUpdate(MeteredSpacePOST viewModel);
        MeteredSpace AddOrUpdate(SensorEventMeteredSpacePOST viewModel);
        MeteredSpaceGET ConvertToViewModel(MeteredSpace entity);
        IQueryable<MeteredSpace> QueryEntities();
        IQueryable<MeteredSpaceGET> QueryViewModels();
    }
}
