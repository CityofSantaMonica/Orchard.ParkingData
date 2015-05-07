using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "lot", Namespace = "")]
    public class ParkingLot
    {
        [DataMember(Name = "available_spaces")]
        public int AvailableSpaces { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "latitude")]
        [Range(-90.0, 90.0)]
        public decimal? Latitude { get; set; }

        [DataMember(Name = "longitude")]
        [Range(-180.0, 180.0)]
        public decimal? Longitude { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "street_address")]
        public string StreetAddress { get; set; }

        [DataMember(Name = "zip_code")]
        public int ZipCode { get; set; }
    }
}
