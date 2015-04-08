using Orchard.ContentManagement;

namespace CSM.ParkingData.Models
{
    public class SensorEventsSettings : ContentPart
    {
        public double LifetimeHours
        {
            get { return this.Retrieve(x => x.LifetimeHours, 3.0); }
            set { this.Store(x => x.LifetimeHours, value); }
        }
    }
}
