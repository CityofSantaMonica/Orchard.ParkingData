using System.Linq;
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
            return _meteredSpacesRepo.Get(m => m.MeterId == meterId);
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
                Active = viewModel.Status.HasValue ? !viewModel.Status.Equals(0) : default(bool?)
            };

            var existing = Get(posted.MeterId);

            if (existing == null)
            {
                _meteredSpacesRepo.Create(posted);
            }
            else
            {
                posted.Id = existing.Id;
                posted.Area = posted.Area ?? existing.Area;
                posted.SubArea = posted.SubArea ?? existing.SubArea;
                posted.Zone = posted.Zone ?? existing.Zone;
                posted.Latitude = posted.Latitude ?? existing.Latitude;
                posted.Longitude = posted.Longitude ?? existing.Longitude;
                posted.Active = posted.Active ?? existing.Active;

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