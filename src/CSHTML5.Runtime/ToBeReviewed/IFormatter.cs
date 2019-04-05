
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#region Assembly mscorlib.dll, v4.0.0.0
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll
#endregion

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
    // Summary:
    //     Provides functionality for formatting serialized objects.
    public interface IFormatter
    {
        // Summary:
        //     Gets or sets the System.Runtime.Serialization.SerializationBinder that performs
        //     type lookups during deserialization.
        //
        // Returns:
        //     The System.Runtime.Serialization.SerializationBinder that performs type lookups
        //     during deserialization.
        SerializationBinder Binder { get; set; }
        //
        // Summary:
        //     Gets or sets the System.Runtime.Serialization.StreamingContext used for serialization
        //     and deserialization.
        //
        // Returns:
        //     The System.Runtime.Serialization.StreamingContext used for serialization
        //     and deserialization.
        StreamingContext Context { get; set; }
        //
        // Summary:
        //     Gets or sets the System.Runtime.Serialization.SurrogateSelector used by the
        //     current formatter.
        //
        // Returns:
        //     The System.Runtime.Serialization.SurrogateSelector used by this formatter.7
        //TODO : can't implemente this. Make sure we don't need this
        //ISurrogateSelector SurrogateSelector { get; set; }

        // Summary:
        //     Deserializes the data on the provided stream and reconstitutes the graph
        //     of objects.
        //
        // Parameters:
        //   serializationStream:
        //     The stream that contains the data to deserialize.
        //
        // Returns:
        //     The top object of the deserialized graph.
        object Deserialize(Stream serializationStream);
        //
        // Summary:
        //     Serializes an object, or graph of objects with the given root to the provided
        //     stream.
        //
        // Parameters:
        //   serializationStream:
        //     The stream where the formatter puts the serialized data. This stream can
        //     reference a variety of backing stores (such as files, network, memory, and
        //     so on).
        //
        //   graph:
        //     The object, or root of the object graph, to serialize. All child objects
        //     of this root object are automatically serialized.
        void Serialize(Stream serializationStream, object graph);
    }
}
