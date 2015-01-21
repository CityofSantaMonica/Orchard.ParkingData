using System;
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

        public MeteredSpace ConvertToEntity(SensorEventMeteredSpacePOST viewModel)
        {
            var existing = _meteredSpacesRepo.GetByMeterId(viewModel.MeterID);

            return existing ?? new MeteredSpace() { MeterId = viewModel.MeterID };
        }

        public MeteredSpace ConvertToEntity(MeteredSpacePOST viewModel)
        {
            return new MeteredSpace() {
                MeterId = viewModel.PoleSerialNumber,
                Area = viewModel.Area,
                SubArea = viewModel.SubArea,
                Zone = viewModel.Zone,
                Latitude = viewModel.Lat,
                Longitude = viewModel.Long,
                IsActive = !viewModel.Status.Equals(0)
            };
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
                IsActive = entity.IsActive
            };
        }

        public IQueryable<MeteredSpace> QueryEntities()
        {
            return _meteredSpacesRepo.Table;
        }

        public IQueryable<MeteredSpaceGET> QueryViewModels()
        {
            return QueryEntities().Select(e => ConvertToViewModel(e));
        }

        public bool TryAddSpace(MeteredSpacePOST viewModel)
        {
            return TryAddSpace(ConvertToEntity(viewModel));
        }

        public bool TryAddSpace(MeteredSpace entity)
        {
            var existing = _meteredSpacesRepo.GetByMeterId(entity.MeterId);

            try
            {
                if (existing == null)
                {
                    _meteredSpacesRepo.Create(entity);
                }
                else
                {
                    entity.Id = existing.Id;
                    _meteredSpacesRepo.Update(entity);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't write MeteredSpace entity to database.");
                return false;
            }
        }
    }
}