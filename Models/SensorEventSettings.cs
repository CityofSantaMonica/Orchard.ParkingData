using Orchard.ContentManagement;

namespace CSM.ParkingData.Models
{

    public class SensorEventsSettings : ContentPart
    {
        public double DefaultLifetimeLength
        {
            get { return this.Retrieve(x => x.DefaultLifetimeLength); }
            set { this.Store(x => x.DefaultLifetimeLength, value); }
        }

        public TimeSpanUnits DefaultLifetimeUnits
        {
            get { return this.Retrieve(x => x.DefaultLifetimeUnits); }
            set { this.Store(x => x.DefaultLifetimeUnits, value); }
        }

        public double MaxLifetimeLength
        {
            get { return this.Retrieve(x => x.MaxLifetimeLength, 3.0); }
            set { this.Store(x => x.MaxLifetimeLength, value); }
        }

        public TimeSpanUnits MaxLifetimeUnits
        {
            get { return this.Retrieve(x => x.MaxLifetimeUnits, TimeSpanUnits.Hours); }
            set { this.Store(x => x.MaxLifetimeUnits, value); }
        }
    }
}
