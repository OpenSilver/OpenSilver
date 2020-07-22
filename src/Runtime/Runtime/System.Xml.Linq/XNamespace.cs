

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


#if !CSHTML5NETSTANDARD
// already defined in .NET Standard
namespace System.Xml.Linq
{
    /// <summary>
    /// Represents an XML namespace. This class cannot be inherited.
    /// </summary>
    public sealed partial class XNamespace
    {
        private static XNamespace _none;

        /// <summary>
        /// Combines an System.Xml.Linq.XNamespace object with a local name to create
        /// an System.Xml.Linq.XName.
        /// </summary>
        /// <param name="ns">An System.Xml.Linq.XNamespace that contains the namespace.</param>
        /// <param name="localName">A System.String that contains the local name.</param>
        /// <returns>The new System.Xml.Linq.XName constructed from the namespace and local name.</returns>
        public static XName operator +(XNamespace ns, string localName)
        {
            return ns.GetName(localName);
        }

        internal const string xmlPrefixNamespace = "http://www.w3.org/XML/1998/namespace";
        internal const string xmlnsPrefixNamespace = "http://www.w3.org/2000/xmlns/";

        /// <summary>
        /// Gets the XNamespace object that corresponds to the xml uri (http://www.w3.org/XML/1998/namespace).
        /// </summary>
        public static XNamespace Xml
        {
            get
            {
                return XNamespace.Get(xmlPrefixNamespace);
            }
        }

        /// <summary>
        /// Gets the XNamespace object that corresponds to the xmlns uri (http://www.w3.org/2000/xmlns/).
        /// </summary>
        public static XNamespace Xmlns
        {
            get
            {
                return XNamespace.Get(xmlnsPrefixNamespace);
            }
        }

        /// <summary>
        /// Gets the <see cref="XNamespace"/> object that corresponds to no namespace.
        /// </summary>
        public static XNamespace None
        {
            get
            {
                if (_none == null)
                {
                    _none = new XNamespace();
                    _none.NamespaceName = string.Empty;
                }
                return _none;
            }
        }


        /// <summary>
        /// Converts a string containing a Uniform Resource Identifier (URI) to an System.Xml.Linq.XNamespace.
        /// </summary>
        /// <param name="namespaceName">A System.String that contains the namespace URI.</param>
        /// <returns>An System.Xml.Linq.XNamespace constructed from the URI string.</returns>
        public static implicit operator XNamespace(string namespaceName)
        {
            return XNamespace.Get(namespaceName);
        }

        string _namespaceName;
        /// <summary>
        /// Gets the Uniform Resource Identifier (URI) of this namespace.
        /// </summary>
        public string NamespaceName
        {
            get
            {
                return _namespaceName;
            }
            internal set
            {
                _namespaceName = value;
            }
        }

        /// <summary>
        /// Gets an System.Xml.Linq.XNamespace for the specified Uniform Resource Identifier
        /// (URI).
        /// </summary>
        /// <param name="namespaceName">A System.String that contains a namespace URI.</param>
        /// <returns>An System.Xml.Linq.XNamespace created from the specified URI.</returns>
        public static XNamespace Get(string namespaceName)
        {
            XNamespace xNamespace = new XNamespace();
            xNamespace.NamespaceName = namespaceName;
            return xNamespace;
        }

        internal static XNamespace Get(string namespaceName, int index, int count)
        {
            return XNamespace.Get(namespaceName.Substring(index, count));
        }

        /// <summary>
        /// Returns an System.Xml.Linq.XName object created from this System.Xml.Linq.XNamespace
        /// and the specified local name.
        /// </summary>
        /// <param name="localName">A System.String that contains a local name.</param>
        /// <returns>
        /// An System.Xml.Linq.XName created from this System.Xml.Linq.XNamespace and
        /// the specified local name.
        /// </returns>
        public XName GetName(string localName)
        {
            XName xName = new XName();
            xName.LocalName = localName;
            xName.Namespace = this;
            return xName;
        }

        internal XName GetName(string localName, int index, int count)
        {
            return this.GetName(localName.Substring(index, count));
        }

        /// <summary>
        /// Returns the URI of this System.Xml.Linq.XNamespace.
        /// </summary>
        /// <returns>The URI of this System.Xml.Linq.XNamespace.</returns>
        public override string ToString()
        {
            return NamespaceName;
        }


        /// <summary>
        /// Returns a value indicating whether two instances of System.Xml.Linq.XNamespace
        /// are not equal.
        /// </summary>
        /// <param name="left">The first System.Xml.Linq.XNamespace to compare.</param>
        /// <param name="right">The second System.Xml.Linq.XNamespace to compare.</param>
        /// <returns>A System.Boolean that indicates whether left and right are not equal.</returns>
        public static bool operator !=(XNamespace left, XNamespace right)
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
            return left._namespaceName != right._namespaceName;
        }

        /// <summary>
        /// Returns a value indicating whether two instances of System.Xml.Linq.XNamespace
        /// are equal.
        /// </summary>
        /// <param name="left">The first System.Xml.Linq.XNamespace to compare.</param>
        /// <param name="right">The second System.Xml.Linq.XNamespace to compare.</param>
        /// <returns>A System.Boolean that indicates whether left and right are equal.</returns>
        public static bool operator ==(XNamespace left, XNamespace right)
        {

            return !(left != right);
        }

        /// <summary>
        /// Gets a hash code for this <see cref="XNamespace"/>.
        /// </summary>
        /// <returns>An <see cref="Int32"/> that contains the hash code for the <see cref="XNamespace"/>.</returns>
        public override int GetHashCode()
        {
            return (this.NamespaceName ?? string.Empty).GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="XNamespace"/> is equal to the
        /// current <see cref="XNamespace"/>.
        /// </summary>
        /// <param name="obj">The <see cref="XNamespace"/> to compare to the current <see cref="XNamespace"/>.</param>
        /// <returns>
        /// A <see cref="Boolean"/> that indicates whether the specified <see cref="XNamespace"/>
        /// is equal to the current <see cref="XNamespace"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this == obj as XNamespace;
        }

        #region not implemented




        ///// <summary>
        ///// Gets the System.Xml.Linq.XNamespace object that corresponds to the XML URI
        ///// (http://www.w3.org/XML/1998/namespace).
        ///// </summary>
        //public static XNamespace Xml { get; }

        ///// <summary>
        ///// Gets the System.Xml.Linq.XNamespace object that corresponds to the xmlns
        ///// URI (http://www.w3.org/2000/xmlns/).
        ///// </summary>
        //public static XNamespace Xmlns { get; } //todo: if we add this, use this instead of DataContractSerializer_Helpers.XMLNS_NAMESPACE?

        #endregion
    }
}
#endif