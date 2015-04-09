using System;
using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Services;
using Orchard.Settings;

namespace CSM.ParkingData.Services
{
    public class SensorEventsService : ISensorEventsService
    {
        private readonly IClock _clock;
        private readonly IMeteredSpacesService _meteredSpacesService;
        private readonly IRepository<SensorEvent> _sensorEventsRepo;
        private readonly ISiteService _siteService;

        public ILogger Logger { get; set; }

        public SensorEventsService(
            IClock clock,
            IMeteredSpacesService meteredSpacesService,
            IRepository<SensorEvent> sensorEventsRepo,
            ISiteService siteService)
        {
            _clock = clock;
            _meteredSpacesService = meteredSpacesService;
            _sensorEventsRepo = sensorEventsRepo;
            _siteService = siteService;

            Logger = NullLogger.Instance;
        }

        public SensorEvent Get(long transmissionId)
        {
            return _sensorEventsRepo.Get(x => x.TransmissionId == transmissionId);
        }

        public SensorEvent AddOrUpdate(SensorEventPOST viewModel)
        {
            var meteredSpace = _meteredSpacesService.AddOrUpdate(viewModel.MeteredSpace);

            var posted = new SensorEvent() {
                ClientId = viewModel.ClientID,
                EventTime = DateTime.Parse(viewModel.EventTime),
                EventType = viewModel.EventType,
                MeteredSpace = meteredSpace,
                ReceivedTime = _clock.UtcNow,
                SessionId = long.Parse(viewModel.MeteredSpace.SessionID),
                TransmissionId = long.Parse(viewModel.TransmissionID),
                TransmissionTime = DateTime.Parse(viewModel.TransmissionDateTime)
            };

            var existing = Get(posted.TransmissionId);

            if (existing == null)
            {
                _sensorEventsRepo.Create(posted);
            }
            else
            {
                posted.Id = existing.Id;
                _sensorEventsRepo.Update(posted);
            }

            return posted;
        }

        public SensorEventGET ConvertToViewModel(SensorEvent entity)
        {
            return new SensorEventGET() {
                EventTime = entity.EventTime,
                EventType = entity.EventType,
                MeterId = entity.MeteredSpace.MeterId,
                ReceivedTime = entity.ReceivedTime,
                SessionId = entity.SessionId,
                TransmissionId = entity.TransmissionId,
            };
        }

        public IQueryable<SensorEvent> Query()
        {
            return _sensorEventsRepo.Table;
        }

        public double GetLifetimeHours()
        {
            var siteSettings = _siteService.GetSiteSettings();
            var sensorEventsSettings = siteSettings.As<SensorEventsSettings>();

            return sensorEventsSettings != null ? sensorEventsSettings.LifetimeHours : 0.0;
        }
    }
}
