using Orchard.ContentManagement;

namespace CSM.ParkingData.Models
{
    public class MeteredSpaceCacheSettings : ContentPart
    {
        public double Length
        {
            get { return this.Retrieve(x => x.Length, 1.0); }
            set { this.Store(x => x.Length, value); }
        }

        public TimeSpanUnits Units
        {
            get { return this.Retrieve(x => x.Units, TimeSpanUnits.Minutes); }
            set { this.Store(x => x.Units, value); }
        }
    }
}
