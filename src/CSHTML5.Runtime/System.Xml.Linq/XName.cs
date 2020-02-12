
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if !CSHTML5NETSTANDARD
// already defined in .NET Standard
namespace System.Xml.Linq
{
    /// <summary>
    /// Represents a name of an XML element or attribute.
    /// </summary>
    public sealed partial class XName : IEquatable<XName>
    {
        /// <summary>
        /// Determines whether the specified System.Xml.Linq.XName is equal to this System.Xml.Linq.XName.
        /// </summary>
        /// <param name="obj">The System.Xml.Linq.XName to compare to the current System.Xml.Linq.XName.</param>
        /// <returns>
        /// true if the specified System.Xml.Linq.XName is equal to the current System.Xml.Linq.XName;
        /// otherwise false.
        /// </returns>
        public override bool Equals(object obj) {
            if (obj is XName)
            {
                return this == (XName)obj;//(this._localName == ((XName)obj).LocalName && this.NamespaceName == ((XName)obj).NamespaceName);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(XName other)
        {
            return this == other;// (this._localName == other.LocalName && this.NamespaceName == other.NamespaceName);
        }

        string _localName;
        /// <summary>
        /// Gets the local (unqualified) part of the name.
        /// </summary>
        public string LocalName
        {
            get { return _localName; }
            internal set { _localName = value; }
        }

        XNamespace _namespace;
        /// <summary>
        /// Gets the namespace part of the fully qualified name.
        /// </summary>
        public XNamespace Namespace
        {
            get
            {
                if (_namespace == null)
                {
                    _namespace = "";
                }
                return _namespace;
            }
            internal set { _namespace = value; }
        }

        /// <summary>
        /// Returns the URI of the System.Xml.Linq.XNamespace for this System.Xml.Linq.XName.
        /// </summary>
        public string NamespaceName
        {
            get
            {
                if (_namespace == null)
                {
                    _namespace = "";
                }
                return _namespace.NamespaceName;
            }
        }

        /// <summary>
        /// Converts a string formatted as an expanded XML name (that is,{namespace}localname)
        /// to an System.Xml.Linq.XName object.
        /// </summary>
        /// <param name="expandedName">A string that contains an expanded XML name in the format {namespace}localname.</param>
        /// <returns>An System.Xml.Linq.XName object constructed from the expanded name.</returns>
        public static implicit operator XName(string expandedName)
        {
            XName xName = new XName();
            string[] splittedName = expandedName.Split('}');
            if (splittedName.Length == 1)
            {
                xName._localName = splittedName[0];
                return xName;
            }
            else if (splittedName.Length == 2)
            {
                xName._namespace = XNamespace.Get(splittedName[0].Substring(1));
                xName._localName = splittedName[1];
                return xName;
            }
            throw new InvalidCastException("the string \"" + expandedName + "\" could not be casted into a XName.");
        }

        /// <summary>
        /// Returns the expanded XML name in the format {namespace}localname.
        /// </summary>
        /// <returns>A System.String that contains the expanded XML name in the format {namespace}localname.</returns>
        public override string ToString()
        {
            string result = string.Empty;
            if (_namespace != null)
            {
                result += "{" + _namespace.NamespaceName + "}";
            }
            result += _localName;
            return result;
        }

        /// <summary>
        /// Returns a value indicating whether two instances of System.Xml.Linq.XName
        /// are not equal.
        /// </summary>
        /// <param name="left">The first System.Xml.Linq.XName to compare.</param>
        /// <param name="right">The second System.Xml.Linq.XName to compare.</param>
        /// <returns>true if left and right are not equal; otherwise false.</returns>
        public static bool operator !=(XName left, XName right)
        {
            if (object.ReferenceEquals(left, null))
            {
                if (object.ReferenceEquals(right, null))
                    return false;
                else
                    return true;
            }
            else if (object.ReferenceEquals(right, null))
            {
                return true;
            }

            if (left.Namespace != right.Namespace)
            {
                return true;
            }
            
            //if we arrive here, we know that their respective namespaces are the same.
            return left._localName != right._localName;
        }

        /// <summary>
        /// Returns a value indicating whether two instances of System.Xml.Linq.XName
        /// are equal.
        /// </summary>
        /// <param name="left">The first System.Xml.Linq.XName to compare.</param>
        /// <param name="right">The second System.Xml.Linq.XName to compare.</param>
        /// <returns>true if left and right are equal; otherwise false.</returns>
        public static bool operator ==(XName left, XName right)
        {
            return !(left != right);
        }








        ///// <summary>
        ///// Gets an System.Xml.Linq.XName object from an expanded name.
        ///// </summary>
        ///// <param name="expandedName">A System.String that contains an expanded XML name in the format {namespace}localname.</param>
        ///// <returns>An System.Xml.Linq.XName object constructed from the expanded name.</returns>
        //public static XName Get(string expandedName);

        ///// <summary>
        ///// Gets an System.Xml.Linq.XName object from a local name and a namespace.
        ///// </summary>
        ///// <param name="localName">A local (unqualified) name.</param>
        ///// <param name="namespaceName">An XML namespace.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XName object created from the specified local name and
        ///// namespace.
        ///// </returns>
        //public static XName Get(string localName, string namespaceName);

        ///// <summary>
        ///// Gets a hash code for this System.Xml.Linq.XName.
        ///// </summary>
        ///// <returns>An System.Int32 that contains the hash code for the System.Xml.Linq.XName.</returns>
        //public override int GetHashCode();
    }
}
#endif