using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CSM.ParkingData.ViewModels;
using Microsoft.WindowsAzure;
using Orchard.Services;

namespace CSM.ParkingData.Services
{
    public class ParkingLotsService : IParkingLotsService
    {
        private readonly IClock _clock;

        public ParkingLotsService(IClock clock)
        {
            _clock = clock;
        }

        public IEnumerable<ParkingLotGET> Get()
        {
            string lotDataUrl = CloudConfigurationManager.GetSetting("ParkingLotDataUrl");
            XDocument xdocument;

            try
            {
                xdocument = XDocument.Load(lotDataUrl);
                return Get(xdocument);
            }
            catch
            {
                return Enumerable.Empty<ParkingLotGET>();
            }
        }

        public IEnumerable<ParkingLotGET> Get(XDocument lotXml)
        {
            var parkingLots = new List<ParkingLotGET>();
            DateTime lastUpdateUtc = ParseLastUpdateUtc(lotXml.Root);

            foreach (var lot in lotXml.Root.Elements("lot"))
            {
                var parkingLot = ParseFromXml(lot);
                parkingLot.LastUpdate = lastUpdateUtc;
                parkingLots.Add(parkingLot);
            }

            return parkingLots;
        }

        public ParkingLotGET ParseFromXml(XElement xml)
        {
            try
            {
                return new ParkingLotGET() {
                    AvailableSpaces = Convert.ToInt32(xml.Element("available").Value),
                    Description = xml.Element("description").Value,
                    Latitude = Convert.ToDecimal(xml.Element("latitude").Value),
                    Longitude = Convert.ToDecimal(xml.Element("longitude").Value),
                    Name = xml.Element("name").Value,
                    StreetAddress = xml.Element("address").Value,
                    ZipCode = Convert.ToInt32(xml.Element("zip").Value)
                };
            }
            catch
            {
                return null;
            }
        }

        public DateTime ParseLastUpdateUtc(XElement xml)
        {
            try
            {
                string dateString = xml.Element("date").Value;
                string timeString = xml.Element("time").Value;

                DateTime local = DateTime.ParseExact(dateString + " " + timeString, "yyyy-MM-dd HH:mm", null);

                return TimeZoneInfo.ConvertTimeToUtc(local, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));
            }
            catch
            {
                return _clock.UtcNow;
            }
        }
    }
}
