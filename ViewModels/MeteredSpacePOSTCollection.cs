using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CSM.ParkingData.ViewModels
{
    [CollectionDataContract(Name = "Poles", Namespace = "")]
    public class MeteredSpacePOSTCollection : List<MeteredSpacePOST>, ISourcedFromXml
    {
    }
}