
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



using System;
using System.Collections;

namespace System.Xml.Serialization
{   
    /// <summary>
    /// Defines the reader, writer, and methods for pre-generated, typed serializers.
    /// </summary>
    public abstract class XmlSerializerImplementation
    {
        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlSerializerImplementation
        /// class.
        /// </summary>
        protected XmlSerializerImplementation()
        {
        }

        /// <summary>
        /// Gets the XML reader object that is used by the serializer.
        /// </summary>
        public virtual XmlSerializationReader Reader
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //not implemented anyway so it's disabled for bridge
#if !BRIDGE
        /// <summary>
        /// Gets the collection of methods that is used to read a data stream.
        /// </summary>
        public virtual Hashtable ReadMethods
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the collection of typed serializers that is found in the assembly.
        /// </summary>
        public virtual Hashtable TypedSerializers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the collection of methods that is used to write to a data stream.
        /// </summary>
        public virtual Hashtable WriteMethods
        {
            get
            {
                throw new NotImplementedException();
            }
        }
#endif

        /// <summary>
        /// Gets the XML writer object for the serializer.
        /// </summary>
        public virtual XmlSerializationWriter Writer { get { throw new NotImplementedException(); } }

        // Summary:
        //     
        //
        // Parameters:
        //   type:
        //     
        //
        // Returns:
        //     
        /// <summary>
        /// Gets a value that determines whether a type can be serialized.
        /// </summary>
        /// <param name="type">The System.Type to be serialized.</param>
        /// <returns>true if the type can be serialized; otherwise, false.</returns>
        public virtual bool CanSerialize(Type type)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     
        //
        // Parameters:
        //   type:
        //     
        //
        // Returns:
        //     
        //     
        /// <summary>
        /// Returns a serializer for the specified type.
        /// </summary>
        /// <param name="type">The System.Type to be serialized.</param>
        /// <returns>
        /// An instance of a type derived from the System.Xml.Serialization.XmlSerializer
        /// class.
        /// </returns>
        public virtual XmlSerializer GetSerializer(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
