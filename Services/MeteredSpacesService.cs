using System;
using System.Collections.Generic;
using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Settings;

namespace CSM.ParkingData.Services
{
    public class MeteredSpacesService : IMeteredSpacesService
    {
        private readonly IRepository<MeteredSpace> _meteredSpacesRepo;
        private readonly ISiteService _siteService;

        public ILogger Logger { get; set; }

        public MeteredSpacesService(
            IRepository<MeteredSpace> meteredSpacesRepo,
            ISiteService siteService)
        {
            _meteredSpacesRepo = meteredSpacesRepo;
            _siteService = siteService;

            Logger = NullLogger.Instance;
        }

        public MeteredSpaceCacheSettings GetCacheSettings()
        {
            var siteSettings = _siteService.GetSiteSettings();
            var meteredSpaceCacheSettings = siteSettings.As<MeteredSpaceCacheSettings>();

            if (meteredSpaceCacheSettings == null)
            {
                meteredSpaceCacheSettings = new MeteredSpaceCacheSettings();
                meteredSpaceCacheSettings.ContentItem = siteSettings.ContentItem;
            }

            return meteredSpaceCacheSettings;
        }

        public MeteredSpace Get(string meterId)
        {
            return _meteredSpacesRepo.Get(m => m.MeterId == meterId);
        }

        public bool Exists(string meterId)
        {
            return Get(meterId) != null;
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
                StreetAddress = entity.SubArea,
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