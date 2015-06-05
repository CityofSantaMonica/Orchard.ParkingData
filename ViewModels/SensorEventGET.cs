using System;
using System.Runtime.Serialization;
using CSM.ParkingData.Extensions;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "sensor_event", Namespace = "")]
    public class SensorEventGET
    {
        [DataMember(Name = "event_id")]
        public long EventId { get; set; }

        public DateTime EventTime { get; set; }

        [DataMember(Name = "event_time")]
        public string EventTimeFormatted
        {
            get { return EventTime.ToIso8061BasicString(); }
            private set { ; }
        }

        [DataMember(Name = "event_type")]
        public string EventType { get; set; }

        [DataMember(Name = "meter_id")]
        public string MeterId { get; set; }

        [DataMember(Name = "session_id")]
        public long SessionId { get; set; }

        [DataMember(Name = "sequence_number")]
        public long SequenceNumber { get; set; }
    }
}
