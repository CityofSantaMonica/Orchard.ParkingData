using System.Runtime.Serialization;
using CSM.ParkingData.Models;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "lifetime", Namespace = "")]
    public class SensorEventLifetime
    {
        [DataMember(Name="length")]
        public double Length { get; set; }

        [DataMember(Name = "scope")]
        public LifetimeScope Scope { get; set; }
    }
}
