using System;
using Moq;
using Orchard.Services;

namespace CSM.ParkingData.Tests
{
    public abstract class TestsBase
    {
        protected static readonly DateTime _dateTimeStub = new DateTime(2015, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        protected Mock<IClock> _mockClock;

        public virtual void SetUp()
        {
            _mockClock = new Mock<IClock>();
            _mockClock
                .Setup(m => m.UtcNow)
                .Returns(_dateTimeStub);
        }
    }
}
