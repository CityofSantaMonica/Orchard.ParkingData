using System.IO;
using System.Runtime.Serialization;
using System.Text;
using CSM.ParkingData.ViewModels;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.MeteredSpaces
{
    public class SerializationTests
    {
        [Test]
        [Category("MeteredSpace")]
        public void Pole_Xml_Deserializes_To_ViewModel()
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
    }
}
