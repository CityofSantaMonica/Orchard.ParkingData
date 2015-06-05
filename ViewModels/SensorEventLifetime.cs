using System;
using System.Runtime.Serialization;
using CSM.ParkingData.Extensions;
using CSM.ParkingData.Models;

namespace CSM.ParkingData.ViewModels
{
    [DataContract(Name = "lifetime", Namespace = "")]
    public class SensorEventLifetime
    {
        public TimeSpanUnits Units { get; set; }

        [DataMember(Name = "length")]
        public double Length { get; set; }

        [DataMember(Name = "units")]
        public string UnitsString
        {
            get { return Units.ToString().ToLower(); }
            private set { ; }
        }

        public DateTime Since { get; set; }

        [DataMember(Name = "since")]
        public string SinceFormatted
        {
            get { return Since.ToIso8061BasicString(); }
            private set { ; }
        }
    }
}
