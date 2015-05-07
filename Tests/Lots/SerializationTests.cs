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

            var lot = new ParkingLot {
                AvailableSpaces = 100,
                Description = "Description here",
                Name = "Lot1",
                Latitude = 42.0M,
                Longitude = -42.0M,
                StreetAddress = "123 Main Street",
                ZipCode = 99999
            };

            //act

            string serialized = JsonConvert.SerializeObject(lot);

            //assert

            StringAssert.IsMatch(@"""available_spaces"":100", serialized);
            StringAssert.IsMatch(@"""description"":""Description here""", serialized);
            StringAssert.IsMatch(@"""name"":""Lot1""", serialized);
            StringAssert.IsMatch(@"""latitude"":42.0", serialized);
            StringAssert.IsMatch(@"""longitude"":-42.0", serialized);
            StringAssert.IsMatch(@"""street_address"":""123 Main Street""", serialized);
            StringAssert.IsMatch(@"""zip_code"":99999", serialized);
        }
    }
}
