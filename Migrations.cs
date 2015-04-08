using System;
using CSM.ParkingData.Models;
using Orchard.Data;
using Orchard.Data.Migration;

namespace CSM.ParkingData
{
    public class Migrations : DataMigrationImpl
    {
        private readonly IRepository<SensorEvent> _sensorEventsRepository;

        public Migrations(IRepository<SensorEvent> sensorEventsRepository)
        {
            _sensorEventsRepository = sensorEventsRepository;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable(
                "SensorEvent",
                table => table
                    .Column<long>("Id", col => col.PrimaryKey().Identity())
                    .Column<string>("ClientId", col => col.NotNull().WithLength(128))
                    .Column<DateTime>("EventTime", col => col.NotNull())
                    .Column<string>("EventType", col => col.NotNull().WithLength(2))
                    .Column<long>("MeteredSpace_Id", col => col.NotNull())
                    .Column<DateTime>("ReceivedTime", col => col.NotNull())
                    .Column<long>("SessionId", col => col.NotNull().WithLength(128))
                    .Column<long>("TransmissionId", col => col.NotNull().Unique())
                    .Column<DateTime>("TransmissionTime", col => col.NotNull())
            );

            var precision = byte.Parse("9");
            var scale = byte.Parse("6");

            SchemaBuilder.CreateTable(
                "MeteredSpace",
                table => table
                    .Column<long>("Id", col => col.PrimaryKey().Identity())
                    .Column<string>("MeterId", col => col.NotNull().Unique().WithLength(128))
                    .Column<string>("Area", col => col.WithLength(128))
                    .Column<string>("SubArea", col => col.WithLength(128))
                    .Column<string>("Zone", col => col.WithLength(128))
                    .Column<decimal>("Latitude", col => col.Nullable().WithPrecision(precision).WithScale(scale))
                    .Column<decimal>("Longitude", col => col.Nullable().WithPrecision(precision).WithScale(scale))
                    .Column<bool>("Active", col => col.Nullable())
            );

            return 2;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(
                "SensorEvent",
                table => table
                    .AddColumn<DateTime>("ReceivedTime")
            );

            foreach (var entity in _sensorEventsRepository.Table)
            {
                entity.ReceivedTime = entity.TransmissionTime;
                _sensorEventsRepository.Update(entity);
            }

            return 2;
        }
    }
}
