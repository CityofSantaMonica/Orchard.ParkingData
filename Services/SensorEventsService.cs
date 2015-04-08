using System;
using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Services;

namespace CSM.ParkingData.Services
{
    public class SensorEventsService : ISensorEventsService
    {
        private readonly IClock _clock;
        private readonly IRepository<SensorEvent> _sensorEventsRepo;
        private readonly IMeteredSpacesService _meteredSpacesService;

        public ILogger Logger { get; set; }

        public SensorEventsService(
            IClock clock,
            IRepository<SensorEvent> sensorEventsRepo,
            IMeteredSpacesService meteredSpacesService)
        {
            _clock = clock;
            _sensorEventsRepo = sensorEventsRepo;
            _meteredSpacesService = meteredSpacesService;

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
                TransmissionTime = entity.TransmissionTime
            };
        }

        public IQueryable<SensorEvent> Query()
        {
            return _sensorEventsRepo.Table;
        }
    }
}
