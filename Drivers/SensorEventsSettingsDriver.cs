using CSM.ParkingData.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace CSM.ParkingData.Drivers
{
    public class SensorEventsSettingsDriver : ContentPartDriver<SensorEventsSettings>
    {
        protected override string Prefix
        {
            get { return "SensorEventsSettings"; }
        }

        protected override DriverResult Editor(SensorEventsSettings part, dynamic shapeHelper)
        {
            return ContentShape(
                "Parts_SensorEvents_Settings",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.SensorEventsSettings",
                    Model: part,
                    Prefix: Prefix
                )
            )
            .OnGroup("Parking API");
        }

        protected override DriverResult Editor(SensorEventsSettings part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
