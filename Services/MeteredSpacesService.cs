using System;
using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;

namespace CSM.ParkingData.Services
{
    public class MeteredSpacesService : IMeteredSpacesService
    {
        public MeteredSpace ConvertToEntity(SensorEventMeteredSpacePOST viewModel)
        {
            throw new NotImplementedException();
        }

        public MeteredSpace ConvertToEntity(MeteredSpacePOST viewModel)
        {
            throw new NotImplementedException();
        }

        public IQueryable<MeteredSpace> QueryEntities()
        {
            throw new NotImplementedException();
        }

        public IQueryable<MeteredSpaceGET> QueryViewModels()
        {
            throw new NotImplementedException();
        }

        public bool TryAddSpace(MeteredSpace entity)
        {
            throw new NotImplementedException();
        }
    }
}