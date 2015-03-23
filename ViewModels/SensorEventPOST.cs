using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "SensorEvent", Namespace = "")]
    public class SensorEventPOST : ISourcedFromXml
    {
        [DataMember(Order = 0)]
        [Required]
        public string ClientID { get; set; }

        [DataMember(Order = 1)]
        [Required]
        public string TransmissionID { get; set; }

        [DataMember(Order = 2)]
        [Required]
        public string TransmissionDateTime { get; set; }

        [DataMember(Order = 3)]
        [Required]
        public string EventType { get; set; }

        [DataMember(Order = 4)]
        [Required]
        public SensorEventMeteredSpacePOST MeteredSpace { get; set; }

        [DataMember(Order = 5)]
        [Required]
        public string EventTime { get; set; }
    }
}