using System;
using System.Collections.Generic;
using System.Xml.Linq;
using CSM.ParkingData.ViewModels;
using Microsoft.WindowsAzure;

namespace CSM.ParkingData.Services
{
    public class ParkingLotsService : IParkingLotsService
    {
        public IEnumerable<ParkingLot> Get()
        {
            var parkingServerUrl = CloudConfigurationManager.GetSetting("ParkingLotServerUrl");
            return Get(parkingServerUrl);
        }

        public IEnumerable<ParkingLot> Get(string parkingServerUrl)
        {
            var xdocument = XDocument.Load(parkingServerUrl);
            var lots = new List<ParkingLot>();

            foreach (var lot in xdocument.Root.Elements("lot"))
            {
                lots.Add(new ParkingLot() {
                    AvailableSpaces = Convert.ToInt32(lot.Element("available").Value),
                    Description = lot.Element("description").Value,
                    Latitude = Convert.ToDecimal(lot.Element("latitude").Value),
                    Longitude = Convert.ToDecimal(lot.Element("longitude").Value),
                    Name = lot.Element("name").Value,
                    StreetAddress = lot.Element("address").Value,
                    ZipCode = Convert.ToInt16(lot.Element("zip").Value)
                });
            }

            return lots;
        }
    }
}
