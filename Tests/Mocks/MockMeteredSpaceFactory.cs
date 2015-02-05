using CSM.ParkingData.Models;
using CSM.ParkingData.Services;
using CSM.ParkingData.ViewModels;
using Moq;

namespace CSM.ParkingData.Tests.Mocks
{
    class MockMeteredSpaceFactory
    {
        public static Mock<IMeteredSpacesService> NewService()
        {
            var mock = new Mock<IMeteredSpacesService>();

            mock
                .Setup(m => m.ConvertToViewModel(It.IsAny<MeteredSpace>()))
                .Returns<MeteredSpace>(
                    ms => new MeteredSpaceGET {
                        Active = ms.Active,
                        MeterId = ms.MeterId
                    }
                );

            return mock;
        }
    }
}
