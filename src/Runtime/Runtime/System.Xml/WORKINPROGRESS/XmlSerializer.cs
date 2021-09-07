using System.IO;

namespace System.Xml.Serialization
{
    public partial class XmlSerializer
    {
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.TextWriter.
        //
        // Parameters:
        //   textWriter:
        //     The System.IO.TextWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        public void Serialize(TextWriter textWriter, object o)
        {
            throw new NotImplementedException("Please use the DataContractSerialiazer instead of the XmlSerializer.");
        }


        // Summary:
        //     Deserializes the XML document contained by the specified System.IO.TextReader.
        //
        // Parameters:
        //   textReader:
        //     The System.IO.TextReader that contains the XML document to deserialize.
        //
        // Returns:
        //     The System.Object being deserialized.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during deserialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public object Deserialize(TextReader textReader)
        {
            return null;
        }
    }
}
