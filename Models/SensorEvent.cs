using System;

namespace CSM.ParkingData.Models
{
    public class SensorEvent
    {
        public virtual long Id { get; set; }

        public virtual long TransmissionId { get; set; }

        public virtual string ClientId { get; set; }

        public virtual long SessionId { get; set; }

        public virtual string EventType { get; set; }

        public virtual DateTime TransmissionTime { get; set; }

        public virtual DateTime EventTime { get; set; }

        public virtual MeteredSpace MeteredSpace { get; set; }
    }
}