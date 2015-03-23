using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using CSM.ParkingData.ViewModels;

namespace CSM.ParkingData.Extensions
{
    public static class XmlSerializationExtensions
    {
        internal static string ToXmlString(this ISourcedFromXml objectGraph)
        {
            DataContractSerializer serializer = new DataContractSerializer(objectGraph.GetType());

            using (var output = new StringWriter())
            using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
            {
                serializer.WriteObject(writer, objectGraph);
                return output.GetStringBuilder().ToString();
            }
        }
    }
}