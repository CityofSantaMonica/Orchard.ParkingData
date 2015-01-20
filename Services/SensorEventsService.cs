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

        public SensorEventsService(
            IRepository<SensorEvent> sensorEventsRepo,
            IMeteredSpacesService meteredSpacesService)
        {
            _sensorEventsRepo = sensorEventsRepo;
            _meteredSpacesService = meteredSpacesService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

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

        public SensorEvent ConvertToEntity(SensorEventPOST viewModel)
        {
            return new SensorEvent() {
                TransmissionId = long.Parse(viewModel.TransmissionID),
                TransmissionTime = DateTime.Parse(viewModel.TransmissionDateTime),
                ClientId = viewModel.ClientID,
                SessionId = long.Parse(viewModel.MeteredSpace.SessionID),
                EventType = viewModel.EventType,
                EventTime = DateTime.Parse(viewModel.EventTime),
                MeteredSpace = _meteredSpacesService.ConvertToEntity(viewModel.MeteredSpace)
            };
        }

        public bool TryAddEvent(SensorEvent entity)
        {
            var existing = _sensorEventsRepo.Get(e => e.TransmissionId == entity.TransmissionId);

            try
            {
                if (existing == null)
                    _sensorEventsRepo.Create(entity);
                else
                    _sensorEventsRepo.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't write entity to database");
                return false;
            }
        }

        public bool TryAddEvent(SensorEventPOST viewModel)
        {
            return TryAddEvent(ConvertToEntity(viewModel));
        }

        public IQueryable<SensorEvent> QueryEntities()
        {
            return _sensorEventsRepo.Table;
        }

        public IQueryable<SensorEventGET> QueryViewModels()
        {
            return QueryEntities().Select(e => ConvertToViewModel(e));
        }
    }
}