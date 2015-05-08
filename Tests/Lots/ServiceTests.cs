using System;
using System.Linq;
using System.Xml.Linq;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;
using NUnit.Framework;
using Orchard.Services;

namespace CSM.ParkingData.Tests.Lots
{
    [TestFixture]
    public class ServiceTests
    {
        Mock<IClock> _mockClock;
        ParkingLotsService _service;

        private XElement newLotXml(string count, string desc, string lat, string lng, string name, string addr, string zip)
        {
            return new XElement("lot", new XElement("available", count),
                                    new XElement("description", desc),
                                    new XElement("latitude", lat),
                                    new XElement("longitude", lng),
                                    new XElement("name", name),
                                    new XElement("address", addr),
                                    new XElement("zip", zip));
        }

        [SetUp]
        public void Setup()
        {
            _mockClock = new Mock<IClock>();
            _service = new ParkingLotsService(_mockClock.Object);
        }

        [Test]
        [Category("Lots")]
        public void Get_ReturnsEmptyCollection_WhenXmlHasNoLots()
        {
            XDocument xml = new XDocument(new XElement("root"));

            var lots = _service.Get(xml);

            Assert.IsNotNull(lots);
            CollectionAssert.IsEmpty(lots);
        }

        [Test]
        [Category("Lots")]
        public void Get_ReturnsParkingLotCollection()
        {
            XDocument lotsXml = new XDocument();
            XElement root = new XElement("root");
            var range = Enumerable.Range(0, 5).Select(i => i.ToString());

            foreach (string i in range)
            {
                root.Add(newLotXml(i, "lot " + i, i, i, "lot " + i, i + " Main Street", "1234" + i));
            }

            lotsXml.Add(root);

            var lots = _service.Get(lotsXml);

            Assert.IsNotNull(lots);
            Assert.AreEqual(5, lots.Count());

            foreach (var lot in lots)
            {
                StringAssert.IsMatch("Main Street", lot.StreetAddress);
            }

            foreach (string i in range)
            {
                Assert.AreEqual(1, lots.Where(lot => lot.AvailableSpaces.ToString() == i).Count());
                Assert.AreEqual(1, lots.Where(lot => lot.Name == "lot " + i.ToString()).Count());
            }
        }

        [Test]
        [Category("Lots")]
        public void ParseFromXml_ReturnsParkingLot()
        {
            XElement xml = newLotXml("100", "somewhere", "42.42", "-42.42", "some lot", "123 Main Street", "12345");

            ParkingLot parkingLot = _service.ParseFromXml(xml);

            Assert.AreEqual(100, parkingLot.AvailableSpaces);
            Assert.AreEqual("somewhere", parkingLot.Description);
            Assert.AreEqual(42.42, parkingLot.Latitude);
            Assert.AreEqual(-42.42, parkingLot.Longitude);
            Assert.AreEqual("some lot", parkingLot.Name);
            Assert.AreEqual("123 Main Street", parkingLot.StreetAddress);
            Assert.AreEqual(12345, parkingLot.ZipCode);
        }

        [Test]
        [Category("Lots")]
        public void ParseFromXml_ReturnsNull_WhenParseFails()
        {
            XElement parkingLotXml =
                new XElement("lot", new XElement("available", "not integer"),
                                    new XElement("latitude", "nope"),
                                    new XElement("longitude", "bad"),
                                    new XElement("zip", "not gonna work"));

            ParkingLot parkingLot = _service.ParseFromXml(parkingLotXml);

            Assert.IsNull(parkingLot);
        }

        [Test]
        [Category("Lots")]
        public void ParseLastUpdateUtc_ReturnsUtcDateTime()
        {
            DateTime testDate = new DateTime();

            XElement dateTimeXml =
                new XElement("datetime", new XElement("date", testDate.ToString("yyyy-MM-dd")),
                                         new XElement("time", testDate.ToString("HH:mm")));

            DateTime lastUpdate = _service.ParseLastUpdateUtc(dateTimeXml);

            Assert.AreEqual(DateTimeKind.Utc, lastUpdate.Kind);
            Assert.AreEqual(testDate.Year, lastUpdate.Year);
            Assert.AreEqual(testDate.Month, lastUpdate.Month);
            Assert.AreEqual(testDate.Day, lastUpdate.Day);

            Assert.AreEqual(testDate.ToUniversalTime().Hour, lastUpdate.Hour);
            Assert.AreEqual(testDate.ToUniversalTime().Minute, lastUpdate.Minute);
        }

        [Test]
        [Category("Lots")]
        public void ParseLastUpdateUtc_ReturnsCurrentUtc_WhenParseFails()
        {
            DateTime testDate = DateTime.UtcNow;

            _mockClock.Setup(m => m.UtcNow).Returns(testDate);

            XElement dateTimeXml =
                new XElement("datetime", new XElement("date", "fail"),
                                         new XElement("time", "bad data"));

            DateTime lastUpdate = _service.ParseLastUpdateUtc(dateTimeXml);

            Assert.AreEqual(testDate, lastUpdate);
        }
    }
}
