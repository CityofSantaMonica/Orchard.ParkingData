using System.Runtime.Serialization;
using CSM.ParkingData.Models;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "lifetime", Namespace = "")]
    public class SensorEventLifetime
    {
        public LifetimeScope Scope { get; set; }

        [DataMember(Name = "length")]
        public double Length { get; set; }

        [DataMember(Name = "scope")]
        public string ScopeString
        {
            get { return Scope.ToString(); }
            private set { ; }
        }
    }
}
