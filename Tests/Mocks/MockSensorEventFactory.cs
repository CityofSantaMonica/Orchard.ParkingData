using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;

namespace CSM.ParkingData.Tests.Mocks
{
    class MockSensorEventFactory
    {
        public static Mock<ISensorEventsService> NewService()
        {
            var mock = new Mock<ISensorEventsService>();

            mock
                .Setup(m => m.ConvertToViewModel(It.IsAny<SensorEvent>()))
                .Returns<SensorEvent>(
                    se => new SensorEventGET {
                        TransmissionId = se.TransmissionId,
                        MeterId = (se.MeteredSpace ?? new MeteredSpace()).MeterId,
                        SessionId = se.SessionId,
                    }
                );

            return mock;
        }
    }
}
