using System;
using System.Linq;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Orchard.Data;
using Orchard.Logging;

namespace CSM.ParkingData.Services
{
    public class SensorEventsService : ISensorEventsService
    {
        private readonly IRepository<SensorEvent> _sensorEventsRepo;
        private readonly IMeteredSpacesService _meteredSpacesService;

        public ILogger Logger { get; set; }

        public SensorEventsService(
            IRepository<SensorEvent> sensorEventsRepo,
            IMeteredSpacesService meteredSpacesService)
        {
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
                TransmissionId = long.Parse(viewModel.TransmissionID),
                TransmissionTime = DateTime.Parse(viewModel.TransmissionDateTime),
                ClientId = viewModel.ClientID,
                SessionId = long.Parse(viewModel.MeteredSpace.SessionID),
                EventType = viewModel.EventType,
                EventTime = DateTime.Parse(viewModel.EventTime),
                MeteredSpace = meteredSpace
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
                TransmissionId = entity.TransmissionId,
                MeterId = entity.MeteredSpace.MeterId,
                SessionId = entity.SessionId,
                TransmissionTime = entity.TransmissionTime,
                EventTime = entity.EventTime,
                EventType = entity.EventType
            };
        }

        public IQueryable<SensorEvent> Query()
        {
            return _sensorEventsRepo.Table;
        }
    }
}