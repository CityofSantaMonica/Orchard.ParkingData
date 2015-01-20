using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "metered_space", Namespace = "")]
    public class MeteredSpaceGET
    {
        [DataMember(Name = "meter_id")]
        public string MeterId { get; set; }

        [DataMember(Name = "area")]
        public string Area { get; set; }

        [DataMember(Name = "sub_area")]
        public string SubArea { get; set; }

        [DataMember(Name = "zone")]
        public string Zone { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "is_active")]
        public bool IsActive { get; set; }
    }
}