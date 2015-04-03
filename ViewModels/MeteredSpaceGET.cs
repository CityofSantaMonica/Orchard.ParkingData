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
        public decimal? Latitude { get; set; }

        [DataMember(Name = "longitude")]
        [Range(-180.0, 180.0)]
        public decimal? Longitude { get; set; }

        [DataMember(Name = "meter_id")]
        public string MeterId { get; set; }

        [DataMember(Name = "street_address")]
        public string StreetAddress { get; set; }
    }
}