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
        public double DefaultLifetimeLength
        {
            get { return this.Retrieve(x => x.DefaultLifetimeLength); }
            set { this.Store(x => x.DefaultLifetimeLength, value); }
        }

        public LifetimeUnits DefaultLifetimeUnits
        {
            get { return this.Retrieve(x => x.DefaultLifetimeUnits); }
            set { this.Store(x => x.DefaultLifetimeUnits, value); }
        }

        public double MaxLifetimeLength
        {
            get { return this.Retrieve(x => x.MaxLifetimeLength, 3.0); }
            set { this.Store(x => x.MaxLifetimeLength, value); }
        }

        public LifetimeUnits MaxLifetimeUnits
        {
            get { return this.Retrieve(x => x.MaxLifetimeUnits, LifetimeUnits.Hours); }
            set { this.Store(x => x.MaxLifetimeUnits, value); }
        }
    }
}
