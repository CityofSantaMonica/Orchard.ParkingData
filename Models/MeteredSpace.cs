namespace CSM.ParkingData.Models
{
    /// <summary>
    /// Storage record for the static data of a parking meter.
    /// </summary>
    public class MeteredSpace
    {
        public virtual long Id { get; set; }

        public virtual string MeterId { get; set; }

        public virtual string Area { get; set; }

        public virtual string SubArea { get; set; }

        public virtual string Zone { get; set; }

        public virtual decimal? Latitude { get; set; }

        public virtual decimal? Longitude { get; set; }

        public virtual bool? Active { get; set; }
    }
}
