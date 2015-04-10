using Orchard.ContentManagement;

namespace CSM.ParkingData.Models
{
    public enum LifetimeUnits
    {
        Hours,
        Minutes,
        Seconds
    }

    public class SensorEventsSettings : ContentPart
    {
        public double LifetimeLength
        {
            get { return this.Retrieve(x => x.LifetimeLength, 3.0); }
            set { this.Store(x => x.LifetimeLength, value); }
        }

        public LifetimeUnits LifetimeUnits
        {
            get { return this.Retrieve(x => x.LifetimeUnits, LifetimeUnits.Hours); }
            set { this.Store(x => x.LifetimeUnits, value); }
        }
    }
}
