using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "MeteredSpace", Namespace = "")]
    public class SensorEventMeteredSpacePOST : ISourcedFromXml
    {
        [DataMember(Order = 0)]
        [Required]
        public string MeterID { get; set; }

        [DataMember(Order = 1)]
        [Required]
        public string SessionID { get; set; }
    }
}
