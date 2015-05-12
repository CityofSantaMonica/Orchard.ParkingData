using System;
using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "sensor_event", Namespace = "")]
    public class SensorEventGET
    {
        [DataMember(Name = "event_id")]
        public long EventId { get; set; }

        [DataMember(Name = "event_time")]
        public DateTime EventTime { get; set; }

        [DataMember(Name = "event_type")]
        public string EventType { get; set; }

        [DataMember(Name = "meter_id")]
        public string MeterId { get; set; }

        [DataMember(Name = "session_id")]
        public long SessionId { get; set; }
    }
}
