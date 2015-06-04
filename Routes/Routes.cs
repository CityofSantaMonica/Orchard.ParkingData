namespace CSM.ParkingData.Routes
{
    internal static class Routes
    {
        public static readonly string Area = "CSM.ParkingData";

        public static readonly string BaseEventRoute = "meters/events";
        public static readonly string BaseEventAtRoute = "meters/{meterId}/events";

        public static readonly int DefaultPriority = 5;

        public static readonly string DateTimeConstraint = @"^\d{8}T\d{6}Z$";
        public static readonly string SequenceConstraint = @"^\d+$";
    }
}
