using Orchard.ContentManagement;

namespace CSM.ParkingData.Models
{
    public class SensorEventsSettings : ContentPart
    {
        public double TimeLimitHours
        {
            get { return this.Retrieve(x => x.TimeLimitHours, 3.0); }
            set { this.Store(x => x.TimeLimitHours, value); }
        }
    }
}
