namespace CSM.ParkingData.Models
{
    public class MeteredSpace
    {
        public virtual long Id { get; set; }

        public virtual string MeterId { get; set; }

        public virtual string Area { get; set; }

        public virtual string SubArea { get; set; }

        public virtual string Zone { get; set; }

        public virtual double Latitude { get; set; }

        public virtual double Longitude { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
