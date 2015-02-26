using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "metered_space", Namespace = "")]
    public class MeteredSpaceGET
    {
        [DataMember(Name = "active")]
        public bool? Active { get; set; }

        [DataMember(Name = "area")]
        public string Area { get; set; }

        [DataMember(Name = "latitude")]
        [Range(-90.0, 90.0)]
        public double? Latitude { get; set; }

        [DataMember(Name = "longitude")]
        [Range(-180.0, 180.0)]
        public double? Longitude { get; set; }

        [DataMember(Name = "meter_id")]
        public string MeterId { get; set; }

        [DataMember(Name = "sub_area")]
        public string SubArea { get; set; }

        [DataMember(Name = "zone")]
        public string Zone { get; set; }
    }
}