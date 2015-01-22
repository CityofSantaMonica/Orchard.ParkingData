using System;
using CSM.ParkingData.Models;
using Orchard.Data;

namespace CSM.ParkingData.Services
{
    public static class RepositoryExtensions
    {
        public static MeteredSpace GetByMeterId(this IRepository<MeteredSpace> repository, string meterId)
        {
            return repository.Get(m => m.MeterId == meterId);
        }

        public static SensorEvent GetByTransmissionId(this IRepository<SensorEvent> repository, long transmissionId)
        {
            return repository.Get(s => s.TransmissionId == transmissionId);
        }
    }
}