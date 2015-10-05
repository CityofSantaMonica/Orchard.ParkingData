using System;
using System.Collections.Generic;
using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard.ContentManagement;
using Orchard.Data;
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
        }

        public SensorEventLifetime GetMaxLifetime()
        {
            var siteSettings = _siteService.GetSiteSettings();
            var sensorEventsSettings = siteSettings.As<SensorEventsSettings>();

            if (sensorEventsSettings == null)
            {
                sensorEventsSettings = new SensorEventsSettings();
                sensorEventsSettings.ContentItem = siteSettings.ContentItem;
            }

            var lifetime = new SensorEventLifetime() {
                Length = sensorEventsSettings.MaxLifetimeLength,
                Units = sensorEventsSettings.MaxLifetimeUnits,
                Since = getLifetimeSince(sensorEventsSettings.MaxLifetimeLength, sensorEventsSettings.MaxLifetimeUnits)
            };

            return lifetime;
        }

        public SensorEventLifetime GetDefaultLifetime()
        {
            var siteSettings = _siteService.GetSiteSettings();
            var sensorEventsSettings = siteSettings.As<SensorEventsSettings>();

            if (sensorEventsSettings == null)
            {
                sensorEventsSettings = new SensorEventsSettings();
                sensorEventsSettings.ContentItem = siteSettings.ContentItem;
            }

            var lifetime = new SensorEventLifetime() {
                Length = sensorEventsSettings.DefaultLifetimeLength,
                Units = sensorEventsSettings.DefaultLifetimeUnits,
                Since = getLifetimeSince(sensorEventsSettings.DefaultLifetimeLength, sensorEventsSettings.DefaultLifetimeUnits)
            };

            return lifetime;
        }

        public SensorEvent AddOrUpdate(SensorEventPOST viewModel)
        {
            //try to get an existing meter, by looking in a local cache first
            var meteredSpace = _meteredSpacesService.Get(viewModel.MeteredSpace.MeterID);
            if (meteredSpace == null)
            {
                //record this meter in the db
                meteredSpace = _meteredSpacesService.AddOrUpdate(viewModel.MeteredSpace);
            }

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

            var existing = _sensorEventsRepo.Get(x => x.TransmissionId == posted.TransmissionId);

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

        public SensorEventGET GetViewModel(SensorEvent entity)
        {
            return new SensorEventGET() {
                EventId = entity.TransmissionId,
                EventTime = entity.EventTime,
                EventType = entity.EventType,
                MeterId = entity.MeteredSpace.MeterId,
                SessionId = entity.SessionId,
                Ordinal = entity.Id,
            };
        }

        public SensorEventGET GetLatestViewModel()
        {
            return GetLatestViewModel(null);
        }

        public SensorEventGET GetLatestViewModel(string meterId)
        {
            var lifetime = GetMaxLifetime();

            return GetViewModelsSince(lifetime.Since, meterId).FirstOrDefault();
        }

        public IQueryable<SensorEvent> Query()
        {
            return _sensorEventsRepo
                    .Table
                    //pre-fetch the metered spaces to speed up later joins
                    .Fetch(s => s.MeteredSpace);
        }

        public IEnumerable<SensorEventGET> GetViewModelsSince(DateTime datetime)
        {
            return GetViewModelsSince(datetime, null);
        }

        public IEnumerable<SensorEventGET> GetViewModelsSince(DateTime datetime, string meterId)
        {
            var query = Query().Where(s => s.EventTime >= datetime);

            if (!String.IsNullOrEmpty(meterId))
                query = query.Where(s => s.MeteredSpace.MeterId == meterId);

            return query.Select(GetViewModel).OrderByDescending(vm => vm.EventTime);
        }

        public IEnumerable<SensorEventGET> GetViewModelsSince(long ordinal)
        {
            return GetViewModelsSince(ordinal, null);
        }

        public IEnumerable<SensorEventGET> GetViewModelsSince(long ordinal, string meterId)
        {
            var maxLifetime = GetMaxLifetime();

            var query = Query().Where(s => s.Id >= ordinal && s.EventTime >= maxLifetime.Since);

            if (!String.IsNullOrEmpty(meterId))
                query = query.Where(s => s.MeteredSpace.MeterId == meterId);

            return query.Select(GetViewModel).OrderByDescending(vm => vm.Ordinal);
        }

        private DateTime getLifetimeSince(double length, TimeSpanUnits units)
        {
            DateTime since = DateTime.MaxValue;
            double lengthModifier = -1 * length;

            switch (units)
            {
                case TimeSpanUnits.Hours:
                    since = _clock.UtcNow.AddHours(lengthModifier);
                    break;
                case TimeSpanUnits.Minutes:
                    since = _clock.UtcNow.AddMinutes(lengthModifier);
                    break;
                case TimeSpanUnits.Seconds:
                    since = _clock.UtcNow.AddSeconds(lengthModifier);
                    break;
            }

            return since;
        }
    }
}
