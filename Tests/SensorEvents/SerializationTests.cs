using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using CSM.ParkingData.ViewModels;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.SensorEvents
{
    public class SerializationTests
    {
        [Test]
        [Category("SensorEvents")]
        public void SensorEvent_Xml_Deserializes_To_ViewModel()
        {
            var serializer = new DataContractSerializer(typeof(SensorEventPOST));

            string xml =
@"<SensorEvent>
<ClientID>XYZ123</ClientID>
<TransmissionID>12345678</TransmissionID>
<TransmissionDateTime>2015-01-26 17:00:00</TransmissionDateTime>
<EventType>SE</EventType>
<MeteredSpace>
<MeterID>Pole1</MeterID>
<SessionID>123</SessionID>
</MeteredSpace>
<EventTime>2015-01-26 17:00:00</EventTime>
</SensorEvent>";

            SensorEventPOST sensorEvent = null;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                sensorEvent = serializer.ReadObject(ms) as SensorEventPOST;
            }

            Assert.NotNull(sensorEvent);
            Assert.AreEqual("XYZ123", sensorEvent.ClientID);
            Assert.AreEqual("12345678", sensorEvent.TransmissionID);
            Assert.AreEqual("2015-01-26 17:00:00", sensorEvent.TransmissionDateTime);
            Assert.AreEqual("SE", sensorEvent.EventType);
            Assert.NotNull(sensorEvent.MeteredSpace);
            Assert.AreEqual("Pole1", sensorEvent.MeteredSpace.MeterID);
            Assert.AreEqual("123", sensorEvent.MeteredSpace.SessionID);
            Assert.AreEqual("2015-01-26 17:00:00", sensorEvent.EventTime);
        }

        [Test]
        [Category("SensorEvents")]
        public void SensorEvent_ViewModel_Serializes_To_Json()
        {
            DateTime time = new DateTime(2015, 1, 26, 17, 0, 0, DateTimeKind.Utc);

            string expected = @"{""transmission_id"":12345678,""meter_id"":""Pole1"",""session_id"":123,""transmission_time"":""2015-01-26T17:00:00Z"",""event_time"":""2015-01-26T17:00:00Z"",""event_type"":""SE""}";

            SensorEventGET viewModel = new SensorEventGET()
            {
                TransmissionId = 12345678,
                MeterId = "Pole1",
                SessionId = 123,
                TransmissionTime = time,
                EventTime = time,
                EventType = "SE"
            };

            string actual = JsonConvert.SerializeObject(viewModel);

            Assert.AreEqual(expected, actual);
        }
    }
}
