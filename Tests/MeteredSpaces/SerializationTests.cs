using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CSM.ParkingData.ViewModels;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.MeteredSpaces
{
    public class SerializationTests
    {
        [Test]
        [Category("MeteredSpaces")]
        public void PoleXml_DeserializesTo_MeteredSpacePOST()
        {
            var serializer = new DataContractSerializer(typeof(MeteredSpacePOST));

            string xml =
@"<Pole>
<Area>WILSHIRE</Area>
<Lat>34.026239</Lat>
<Long>-118.489714</Long>
<PoleSerialNumber>WIL1301</PoleSerialNumber>
<Status>1</Status>
<SubArea>1301 WILSHIRE BLVD</SubArea>
<Zone>Santa Monica, CA Default Zone</Zone>
</Pole>
";
            MeteredSpacePOST pole = null;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                pole = serializer.ReadObject(ms) as MeteredSpacePOST;
            }

            Assert.NotNull(pole);
            Assert.AreEqual("WILSHIRE", pole.Area);
            Assert.AreEqual(34.026239, pole.Lat);
            Assert.AreEqual(-118.489714, pole.Long);
            Assert.AreEqual("WIL1301", pole.PoleSerialNumber);
            Assert.AreEqual(1, pole.Status);
            Assert.AreEqual("1301 WILSHIRE BLVD", pole.SubArea);
            Assert.AreEqual("Santa Monica, CA Default Zone", pole.Zone);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void PolesCollectionXml_DeserializesTo_MeteredSpacePOSTCollection()
        {
            var serializer = new DataContractSerializer(typeof(MeteredSpacePOSTCollection));

            string xml =
@"<Poles>
<Pole>
<Area>WILSHIRE</Area>
<Lat>34.026239</Lat>
<Long>-118.489714</Long>
<PoleSerialNumber>WIL1301</PoleSerialNumber>
<Status>1</Status>
<SubArea>1301 WILSHIRE BLVD</SubArea>
<Zone>Santa Monica, CA Default Zone</Zone>
</Pole>
<Pole>
<Area>WILSHIRE</Area>
<Lat>34.026239</Lat>
<Long>-118.489715</Long>
<PoleSerialNumber>WIL1302</PoleSerialNumber>
<Status>1</Status>
<SubArea>1302 WILSHIRE BLVD</SubArea>
<Zone>Santa Monica, CA Default Zone</Zone>
</Pole>
</Poles>
";
            MeteredSpacePOSTCollection poles = null;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                poles = serializer.ReadObject(ms) as MeteredSpacePOSTCollection;
            }

            Assert.NotNull(poles);
            Assert.AreEqual(2, poles.Count);
            Assert.AreEqual("WIL1301", poles.First().PoleSerialNumber);
            Assert.AreEqual("WIL1302", poles.Last().PoleSerialNumber);
        }

        [Test]
        [Category("MeteredSpaces")]
        public void MeteredSpaceGET_SerializesToJson()
        {
            //arrange

            string expected = @"{""active"":true,""area"":""51"",""latitude"":42.0,""longitude"":-42.0,""meter_id"":""Pole1"",""sub_area"":""255"",""zone"":""Friend""}";

            var viewModel = new MeteredSpaceGET {
                Active = true,
                Area = "51",
                Latitude = 42.0,
                Longitude = -42.0,
                MeterId = "Pole1",
                SubArea = "255",
                Zone = "Friend"
            };

            //act

            string actual = JsonConvert.SerializeObject(viewModel);

            //assert

            Assert.AreEqual(expected, actual);
        }
    }
}
