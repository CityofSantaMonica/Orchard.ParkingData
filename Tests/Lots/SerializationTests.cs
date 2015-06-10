using System;
using CSM.ParkingData.ViewModels;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.Lots
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        [Category("Lots")]
        public void ParkingLot_SerializesToJson()
        {
            //arrange

            var lot = new ParkingLotGET {
                AvailableSpaces = 100,
                Description = "Description here",
                Name = "Lot 1",
                LastUpdate = new DateTime(2015, 5, 8, 15, 0, 0, DateTimeKind.Utc), 
                Latitude = 42.0M,
                Longitude = -42.0M,
                StreetAddress = "123 Main Street",
                ZipCode = 99999
            };

            //the computed Id should match *before* serialization
            Assert.AreEqual(lot.Id, 7649);

            //act

            string serialized = JsonConvert.SerializeObject(lot);

            //assert

            StringAssert.IsMatch(@"""available_spaces"":100", serialized);
            StringAssert.IsMatch(@"""description"":""Description here""", serialized);
            StringAssert.IsMatch(@"""name"":""Lot 1""", serialized);
            StringAssert.IsMatch(@"""id"":7649", serialized);
            StringAssert.IsMatch(@"""last_update"":""20150508T150000Z""", serialized);
            StringAssert.IsMatch(@"""latitude"":42.0", serialized);
            StringAssert.IsMatch(@"""longitude"":-42.0", serialized);
            StringAssert.IsMatch(@"""street_address"":""123 Main Street""", serialized);
            StringAssert.IsMatch(@"""zip_code"":99999", serialized);
        }
    }
}
