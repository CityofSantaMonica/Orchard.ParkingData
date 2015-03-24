using System;
using CSM.ParkingData.Models;
using Orchard.Data.Migration;

namespace CSM.ParkingData
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                typeof(SensorEvent).Name,
                table => table
                    .Column<long>("Id", col => col.PrimaryKey().Identity())
                    .Column<long>("TransmissionId", col => col.NotNull().Unique())
                    .Column<string>("ClientId", col => col.NotNull().WithLength(128))
                    .Column<long>("SessionId", col => col.NotNull().WithLength(128))
                    .Column<string>("EventType", col => col.NotNull().WithLength(2))
                    .Column<DateTime>("TransmissionTime", col => col.NotNull())
                    .Column<DateTime>("EventTime", col => col.NotNull())
                    .Column<long>("MeteredSpace_Id", col => col.NotNull())
            );

            var precision = byte.Parse("9");
            var scale = byte.Parse("6");

            SchemaBuilder.CreateTable(
                typeof(MeteredSpace).Name,
                table => table
                    .Column<long>("Id", col => col.PrimaryKey().Identity())
                    .Column<string>("MeterId", col => col.NotNull().Unique())
                    .Column<string>("Area", col => col.WithLength(128))
                    .Column<string>("SubArea", col => col.WithLength(128))
                    .Column<string>("Zone", col => col.WithLength(128))
                    .Column<decimal>("Latitude", col => col.Nullable().WithPrecision(precision).WithScale(scale))
                    .Column<decimal>("Longitude", col => col.Nullable().WithPrecision(precision).WithScale(scale))
                    .Column<bool>("Active", col => col.Nullable())
            );

            return 1;
        }
    }
}