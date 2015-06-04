using CSM.ParkingData.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace CSM.ParkingData.Drivers
{
    public class MeteredSpaceCacheSettingsDriver : ContentPartDriver<MeteredSpaceCacheSettings>
    {
        protected override string Prefix
        {
            get { return "MeteredSpaceCacheSettings"; }
        }

        protected override DriverResult Editor(MeteredSpaceCacheSettings part, dynamic shapeHelper)
        {
            return ContentShape(
                "Parts_MeteredSpaceCacheSettings",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts.MeteredSpaceCacheSettings",
                    Model: part,
                    Prefix: Prefix
                )
            )
            .OnGroup("Parking API");
        }

        protected override DriverResult Editor(MeteredSpaceCacheSettings part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
