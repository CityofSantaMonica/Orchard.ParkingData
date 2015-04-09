using Orchard.ContentManagement;

namespace CSM.ParkingData.Models
{
    public enum LifetimeScope
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

        public LifetimeScope LifetimeScope
        {
            get { return this.Retrieve(x => x.LifetimeScope, LifetimeScope.Hours); }
            set { this.Store(x => x.LifetimeScope, value); }
        }
    }
}
