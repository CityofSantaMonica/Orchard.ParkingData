using CSM.ParkingData.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace CSM.ParkingData.Handlers
{
    public class SensorEventsSettingsHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public SensorEventsSettingsHandler()
        {
            T = NullLocalizer.Instance;

            Filters.Add(new ActivatingFilter<SensorEventsSettings>("Site"));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType != "Site")
                return;

            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Parking API")));
        }
    }
}