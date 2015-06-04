using CSM.ParkingData.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace CSM.ParkingData.Handlers
{
    public class MeteredSpaceCacheSettingsHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public MeteredSpaceCacheSettingsHandler()
        {
            T = NullLocalizer.Instance;

            Filters.Add(new ActivatingFilter<MeteredSpaceCacheSettings>("Site"));
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