
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

namespace System.Xml
{
    /// <summary>
    /// Represents an XML qualified name.
    /// </summary>
    [Serializable]
#if !BRIDGE
    [JSIL.Meta.JSStubOnly]
#endif
    public class XmlQualifiedName
    {
        /// <summary>
        /// Provides an empty System.Xml.XmlQualifiedName.
        /// </summary>
        public static readonly XmlQualifiedName Empty;

        /// <summary>
        /// Initializes a new instance of the System.Xml.XmlQualifiedName class.
        /// </summary>
        public XmlQualifiedName()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Xml.XmlQualifiedName class with
        /// the specified name.
        /// </summary>
        /// <param name="name">The local name to use as the name of the System.Xml.XmlQualifiedName object.</param>
        public XmlQualifiedName(string name)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Xml.XmlQualifiedName class with
        /// the specified name and namespace.
        /// </summary>
        /// <param name="name">The local name to use as the name of the System.Xml.XmlQualifiedName object.</param>
        /// <param name="ns">The namespace for the System.Xml.XmlQualifiedName object.</param>
        public XmlQualifiedName(string name, string ns)
        {
            throw new NotImplementedException();
        }

       
        /// <summary>
        /// Compares two System.Xml.XmlQualifiedName objects.
        /// </summary>
        /// <param name="a">An System.Xml.XmlQualifiedName to compare.</param>
        /// <param name="b">An System.Xml.XmlQualifiedName to compare.</param>
        /// <returns>
        /// true if the name and namespace values for the two objects differ; otherwise,
        /// false.
        /// </returns>
        public static bool operator !=(XmlQualifiedName a, XmlQualifiedName b)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Compares two System.Xml.XmlQualifiedName objects.
        /// </summary>
        /// <param name="a">An System.Xml.XmlQualifiedName to compare.</param>
        /// <param name="b">An System.Xml.XmlQualifiedName to compare.</param>
        /// <returns>
        /// true if the two objects have the same name and namespace values; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(XmlQualifiedName a, XmlQualifiedName b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the System.Xml.XmlQualifiedName is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// Gets a string representation of the qualified name of the System.Xml.XmlQualifiedName.
        /// </summary>
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }
       
        /// <summary>
        /// Gets a string representation of the namespace of the System.Xml.XmlQualifiedName.
        /// </summary>
        public string Namespace
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Determines whether the specified System.Xml.XmlQualifiedName object is equal
        /// to the current System.Xml.XmlQualifiedName object.
        /// </summary>
        /// <param name="other">The System.Xml.XmlQualifiedName to compare.</param>
        /// <returns>true if the two are the same instance object; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Returns the hash code for the System.Xml.XmlQualifiedName.
        /// </summary>
        /// <returns>A hash code for this object.</returns>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
       
        /// <summary>
        /// Returns the string value of the System.Xml.XmlQualifiedName.
        /// </summary>
        /// <returns>
        /// The string value of the System.Xml.XmlQualifiedName in the format of namespace:localname.
        /// If the object does not have a namespace defined, this method returns just
        /// the local name.
        /// </returns>
        public override string ToString()
        {
            throw new NotImplementedException();
        }
     
        /// <summary>
        /// Returns the string value of the System.Xml.XmlQualifiedName.
        /// </summary>
        /// <param name="name">The name of the object.</param>
        /// <param name="ns">The namespace of the object.</param>
        /// <returns>
        /// The string value of the System.Xml.XmlQualifiedName in the format of namespace:localname.
        /// If the object does not have a namespace defined, this method returns just
        /// the local name.
        /// </returns>
        public static string ToString(string name, string ns)
        {
            throw new NotImplementedException();
        }
    }
}
