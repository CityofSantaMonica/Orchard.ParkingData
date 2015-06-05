using System;
using CSM.ParkingData.Models;
using CSM.ParkingData.ViewModels;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CSM.ParkingData.Tests.Lifetime
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        [Category("SensorEventsLifetime")]
        public void SensorEventLifetime_SerializesToJson()
        {
            //arrange

            DateTime since = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var lifetime1 = new SensorEventLifetime() { Length = 1.0, Units = TimeSpanUnits.Hours, Since = since };
            var lifetime2 = new SensorEventLifetime() { Length = 0.42, Units = TimeSpanUnits.Seconds, Since = since };

            //act

            string actual1 = JsonConvert.SerializeObject(lifetime1);
            string actual2 = JsonConvert.SerializeObject(lifetime2);

            //assert

            StringAssert.IsMatch(@"""length"":1.0", actual1);
            StringAssert.IsMatch(@"""units"":""hours""", actual1);
            StringAssert.IsMatch(@"""since"":""2015-01-01T00:00:00Z""", actual1);

            StringAssert.IsMatch(@"""length"":0.42", actual2);
            StringAssert.IsMatch(@"""units"":""seconds""", actual2);
            StringAssert.IsMatch(@"""since"":""2015-01-01T00:00:00Z""", actual2);
        }
    }
}
