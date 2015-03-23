using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "Pole", Namespace = "")]
    public class MeteredSpacePOST : ISourcedFromXml
    {
        [DataMember(Order = 0)]
        [Required]
        public string Area { get; set; }

        [DataMember(Order = 1)]
        [Required]
        public double? Lat { get; set; }

        [DataMember(Order = 2)]
        [Required]
        public double? Long { get; set; }

        [DataMember(Order = 3)]
        [Required]
        public string PoleSerialNumber { get; set; }

        [DataMember(Order = 4)]
        [Required]
        [Range(0,1)]
        public int? Status { get; set; }

        [DataMember(Order = 5)]
        [Required]
        public string SubArea { get; set; }

        [DataMember(Order = 6)]
        [Required]
        public string Zone { get; set; }
    }
}