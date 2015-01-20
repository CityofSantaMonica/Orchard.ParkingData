using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CSM.ParkingData.Attributes;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "sensor_event", Namespace = "")]
    [ApiDocumentation(
        descriptionLines: new[] { 
            "A sensor_event is recorded each time a vehicle pulls into or out of one of the metered parking spaces in Santa Monica.",
            "Corresponding SS and SE sensor_events share a common session_id."
        }
    )]
    public class SensorEventGET
    {
        [DataMember(Name = "transmission_id")]
        [ApiDocumentation(
            dataType: "integer",
            descriptionLines: "unique identifier for an event."
        )]
        public long TransmissionId { get; set; }

        [DataMember(Name = "meter_id")]
        [ApiDocumentation(
            dataType: "string",
            descriptionLines: "unique identifier for the metered space at which an event occured."
        )]
        public string MeterId { get; set; }

        [DataMember(Name = "session_id")]
        [ApiDocumentation(
            dataType: "integer",
            descriptionLines: "identifier tying a particular SS event to the corresponding SE event."
        )]
        public long SessionId { get; set; }

        [DataMember(Name = "transmission_time")]
        [ApiDocumentation(
            dataType: "datetime (UTC)",
            descriptionLines: "the date and time an event's data is transmitted."
        )]
        public DateTime TransmissionTime { get; set; }

        [DataMember(Name = "event_time")]
        [ApiDocumentation(
            dataType: "datetime (UTC)",
            descriptionLines: "the date and time an event is recorded by the meter."
        )]
        public DateTime EventTime { get; set; }

        [DataMember(Name = "event_type")]
        [ApiDocumentation(
            dataType: "SS",
            descriptionLines: "(Session start) indicates the event corresponds to a vehicle pulling into a metered parking space."
        )]
        [ApiDocumentation(
            dataType: "SE",
            descriptionLines: "(Session end) indicates the event corresponds to a vehicle pulling out of a metered parking space."
        )]
        public string EventType { get; set; }
    }
}