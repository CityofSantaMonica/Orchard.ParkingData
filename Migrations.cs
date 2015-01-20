using System;
using CSM.ParkingData.Models;
using Orchard.Data;
using Orchard.Data.Migration;

namespace CSM.ParkingData.Data
{
    public class Migrations : DataMigrationImpl
    {
        private readonly IRepository<SensorEvent> _sensorEventsRepository;
        private readonly IRepository<MeteredSpace> _meteredSpacesRepository;

        public Migrations(
            IRepository<SensorEvent> sensorEventsRepository,
            IRepository<MeteredSpace> meteredSpacesRepository)
        {
            _sensorEventsRepository = sensorEventsRepository;
            _meteredSpacesRepository = meteredSpacesRepository;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable(
                typeof(SensorEvent).Name,
                table => table
                    .Column<long>("Id", col => col.PrimaryKey().Identity())
                    .Column<long>("TransmissionId", col => col.NotNull().Unique())
                    .Column<string>("ClientId", col => col.WithLength(512).NotNull())
                    .Column<long>("SessionId", col => col.NotNull())
                    .Column<string>("EventType", col => col.WithLength(2).NotNull())
                    .Column<DateTime>("TransmissionTime", col => col.NotNull())
                    .Column<DateTime>("EventTime", col => col.NotNull())
                    .Column<string>("MeteredSpace_Id", col => col.NotNull())
            );

            SchemaBuilder.CreateTable(
                typeof(MeteredSpace).Name,
                table => table
                    .Column<long>("Id", col => col.PrimaryKey().Identity())
                    .Column<string>("MeterId", col => col.NotNull().Unique())
                    .Column<string>("Area", col => col.WithLength(512).NotNull())
                    .Column<string>("SubArea", col => col.WithLength(512).NotNull())
                    .Column<string>("Zone", col => col.WithLength(512).NotNull())
                    .Column<double>("Latitude", col => col.NotNull())
                    .Column<double>("Longitude", col => col.NotNull())
                    .Column<bool>("IsActive", col => col.NotNull())
            );

            _meteredSpacesRepository.Create(new MeteredSpace {
                MeterId = "WIL1301",
                Area = "WILSHIRE",
                SubArea = "1301 WILSHIRE BLVD",
                Zone = "Santa Monica, CA Default Zone",
                Latitude = 34.026239,
                Longitude = -118.489714,
                IsActive = true
            });

            _meteredSpacesRepository.Create(new MeteredSpace {
                MeterId = "BAR2813",
                Area = "BEACH",
                SubArea = "2801 BARNARD WY",
                Zone = "Santa Monica, CA Default Zone",
                Latitude = 33.998598,
                Longitude = -118.483524,
                IsActive = false
            });

            var meteredSpace = _meteredSpacesRepository.Get(m => m.MeterId == "WIL1301");

            _sensorEventsRepository.Create(new SensorEvent {
                TransmissionId = 0L,
                ClientId = "XYZ123",
                SessionId = 12345L,
                EventType = "SS",
                EventTime = DateTime.Now,
                TransmissionTime = DateTime.Now,
                MeteredSpace = meteredSpace
            });
            
            _sensorEventsRepository.Create(new SensorEvent {
                TransmissionId = 1L,
                ClientId = "XYZ123",
                SessionId = 12345L,
                EventType = "SE",
                EventTime = DateTime.Now,
                TransmissionTime = DateTime.Now,
                MeteredSpace = meteredSpace
            });

            

            return 1;
        }
    }
}