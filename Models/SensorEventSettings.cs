using Orchard.ContentManagement;

namespace CSM.ParkingData.Models
{
    public class SensorEventsSettings : ContentPart
    {
        public int TimeLimitHours
        {
            get { return this.Retrieve(x => x.TimeLimitHours, 3); }
            set { this.Store(x => x.TimeLimitHours, value); }
        }
    }
}
