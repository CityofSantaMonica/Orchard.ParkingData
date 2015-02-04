using System.Linq;
using CSM.ParkingData.Helpers;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard.Data;
using Orchard.Logging;

namespace CSM.ParkingData.Services
{
    public class MeteredSpacesService : IMeteredSpacesService
    {
        private readonly IRepository<MeteredSpace> _meteredSpacesRepo;

        public ILogger Logger { get; set; }

        public MeteredSpacesService(IRepository<MeteredSpace> meteredSpacesRepo)
        {
            _meteredSpacesRepo = meteredSpacesRepo;

            Logger = NullLogger.Instance;
        }

        public MeteredSpace Get(string meterId)
        {
            return _meteredSpacesRepo.GetByMeterId(meterId);
        }

        public MeteredSpace AddOrUpdate(MeteredSpacePOST viewModel)
        {
            var posted = new MeteredSpace() {
                MeterId = viewModel.PoleSerialNumber,
                Area = viewModel.Area,
                SubArea = viewModel.SubArea,
                Zone = viewModel.Zone,
                Latitude = viewModel.Lat,
                Longitude = viewModel.Long,
                Active = !viewModel.Status.Equals(0)
            };

            var existing = _meteredSpacesRepo.GetByMeterId(posted.MeterId);

            if (existing == null)
            {
                _meteredSpacesRepo.Create(posted);
            }
            else
            {
                posted.Id = existing.Id;
                _meteredSpacesRepo.Update(posted);
            }

            return posted;
        }

        public MeteredSpace AddOrUpdate(SensorEventMeteredSpacePOST viewModel)
        {
            return AddOrUpdate(new MeteredSpacePOST() { PoleSerialNumber = viewModel.MeterID });
        }

        public MeteredSpaceGET ConvertToViewModel(MeteredSpace entity)
        {
            return new MeteredSpaceGET() {
                MeterId = entity.MeterId,
                Area = entity.Area,
                SubArea = entity.SubArea,
                Zone = entity.Zone,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Active = entity.Active
            };
        }

        public IQueryable<MeteredSpace> Query()
        {
            return _meteredSpacesRepo.Table;
        }
    }
}