using System;
using CSM.ParkingData.Extensions;
using NUnit.Framework;

namespace CSM.ParkingData.Tests
{
    [TestFixture]
    public class Iso8061BasicExtensionsTests
    {
        [Test]
        [Category("Iso8061Basic")]
        public void ToIso8061BasicString_ReturnsStringInExpectedFormat()
        {
            DateTime stub = new DateTime(2015, 06, 05, 16, 45, 0, DateTimeKind.Utc);
            string expected = "20150605T164500Z";

            Assert.AreEqual(expected, stub.ToIso8061BasicString());
        }

        [Test]
        [Category("Iso8061Basic")]
        public void ToIso8061BasicString_ConvertsToUtcDateTime()
        {
            DateTime stub = new DateTime(2015, 06, 05, 9, 45, 0, DateTimeKind.Local);
            string expected = "20150605T164500Z";

            Assert.AreEqual(expected, stub.ToIso8061BasicString());
        }

        [Test]
        [Category("Iso8061Basic")]
        public void IsIso8061Basic_ReturnsTrue_WhenGoodFormat()
        {
            string stub = "20150605T164500Z";

            Assert.True(stub.IsIso8061Basic());
        }

        //nothing
        [TestCase((string)null)]
        [TestCase("")]
        //not even close
        [TestCase("not a date")]
        [TestCase("January 1, 2015 12:00 A.M.")]
        //date no time
        [TestCase("20150101TZ")]
        //time no date
        [TestCase("T000000Z")]
        //date incomplete time
        [TestCase("20150101T00Z")]
        [TestCase("20150101T0000Z")]
        //time incomplete date
        [TestCase("2015T000000Z")]
        [TestCase("201501T000000Z")]
        //no UTC specifier
        [TestCase("20150101T000000")]
        [Category("Iso8061Basic")]
        public void IsIso8061Basic_ReturnsFalse_WhenBadFormat(string badFormattedString)
        {

            Assert.False(badFormattedString.IsIso8061Basic());
        }

        [Test]
        [Category("Iso8061Basic")]
        public void TryParseIso8061Basic_ReturnsTrueAndParsesToUtcDateTime_WhenGoodFormat()
        {
            string stub = "20150605T164500Z";
            DateTime expected = new DateTime(2015, 06, 05, 16, 45, 0, DateTimeKind.Utc);
            DateTime parsed;

            stub.TryParseIso8061Basic(out parsed);

            Assert.AreEqual(expected, parsed);
        }

        //nothing
        [TestCase((string)null)]
        [TestCase("")]
        //not even close
        [TestCase("not a date")]
        [TestCase("January 1, 2015 12:00 A.M.")]
        //date no time
        [TestCase("20150101TZ")]
        //time no date
        [TestCase("T000000Z")]
        //date incomplete time
        [TestCase("20150101T00Z")]
        [TestCase("20150101T0000Z")]
        //time incomplete date
        [TestCase("2015T000000Z")]
        [TestCase("201501T000000Z")]
        //no UTC specifier
        [TestCase("20150101T000000")]
        [Category("Iso8061Basic")]
        public void TryParseIso8061Basic_ReturnsFalse_WhenBadFormat(string badFormattedString)
        {
            DateTime parsed = DateTime.MinValue;

            Assert.False(badFormattedString.TryParseIso8061Basic(out parsed));
            Assert.AreEqual(DateTime.MinValue, parsed);
        }
    }
}
