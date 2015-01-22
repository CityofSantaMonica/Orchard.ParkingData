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
                    .Column<string>("Area", col => col.WithLength(512))
                    .Column<string>("SubArea", col => col.WithLength(512))
                    .Column<string>("Zone", col => col.WithLength(512))
                    .Column<double>("Latitude")
                    .Column<double>("Longitude")
                    .Column<bool>("IsActive", col => col.NotNull())
            );

            return 1;
        }
    }
}