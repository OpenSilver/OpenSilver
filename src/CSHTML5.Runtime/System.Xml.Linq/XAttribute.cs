
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



using CSHTML5.Internal;
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
    /// Represents an XML attribute.
    /// </summary>
    public class XAttribute : XObject
    {
        private bool isJsValueUpToDate = false;

        /// <summary>
        /// Note: it is up to the element that sets this to handle the corresponding actions (adding the attribute and setting the value when setting it, removing the attribute when unsetting it).
        /// </summary>
        internal object INTERNAL_containerJSNode;

        //todo: (?) add internal methods that:
        //          set the INTERNAL_containerJSNode and add and initialize the attribute in the js
        //          unset the INTERNAL_containerJSNode and remove the attribute in the js

        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XAttribute class from another
        /// System.Xml.Linq.XAttribute object.
        /// </summary>
        /// <param name="other">An System.Xml.Linq.XAttribute object to copy from.</param>
        public XAttribute(XAttribute other)
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }
            _name = other.Name;
            _value = other.Value;
            _nameWithPrefix = other._nameWithPrefix;
        }


        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XAttribute class from the
        /// specified name and value.
        /// </summary>
        /// <param name="name">The System.Xml.Linq.XName of the attribute.</param>
        /// <param name="value">An System.Object containing the value of the attribute.</param>
        public XAttribute(XName name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("The argument \"name\" cannot be null.");
            }
            if (value == null)
            {
                throw new ArgumentNullException("The argument \"value\" cannot be null.");
            }
            _name = name;
            _value = value.ToString();
            _nameWithPrefix = name;
        }

        internal XAttribute(XName name, object value, XName nameWithPrefix) : this(name, value)
        {
            _nameWithPrefix = nameWithPrefix;
        }

        internal XName _nameWithPrefix = null;

        XName _name;
        /// <summary>
        /// Gets the expanded name of this attribute.
        /// </summary>
        public XName Name { get { return _name; } }

        string _value; //this is necessary because value can be set before adding the attribute to an XElement/XContainer
        ///// <summary>
        ///// Gets the next attribute of the parent element.
        ///// </summary>
        //public XAttribute NextAttribute { get; }

        //// Returns:
        ////     The node type. For System.Xml.Linq.XAttribute objects, this value is System.Xml.XmlNodeType.Attribute.
        ///// <summary>
        ///// Gets the node type for this node.
        ///// </summary>
        //public override XmlNodeType NodeType { get; }

        ///// <summary>
        ///// Gets the previous attribute of the parent element.
        ///// </summary>
        //public XAttribute PreviousAttribute { get; }


        /// <summary>
        /// Gets or sets the value of this attribute.
        /// </summary>
        public string Value
        {
            get
            {
                if (INTERNAL_containerJSNode != null && isJsValueUpToDate)
                {
                    object attributejs;
                    if (string.IsNullOrWhiteSpace(Name.NamespaceName))
                    {
                        attributejs = CSHTML5.Interop.ExecuteJavaScript("$0.getAttribute($1)", INTERNAL_containerJSNode, Name.LocalName);
                    }
                    else
                    {
                        attributejs = CSHTML5.Interop.ExecuteJavaScript("$0.getAttributeNS($1,$2)", INTERNAL_containerJSNode, Name.NamespaceName, Name.LocalName);
                    }
                    if (Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0 != undefined && $0 != null", attributejs)))
                    {
                        _value = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0", attributejs)); //this lets us keep an up to date value.
                    }
                    //an "else" is possible when we have removed the attribute from the element using another instance of XAttribute pointing to the same one.
                    //todo: else what do we do ?
                }
                return _value;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                isJsValueUpToDate = false;
                if (INTERNAL_containerJSNode != null)
                {
                    if (string.IsNullOrWhiteSpace(Name.NamespaceName))
                    {
                        CSHTML5.Interop.ExecuteJavaScriptAsync("$0.setAttribute($1, $2)", INTERNAL_containerJSNode, Name.LocalName, value);
                    }
                    else
                    {
                        //we try to find a prefix for the namespace and if there is one, we use it:
                        string str = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.lookupPrefix($1)", INTERNAL_containerJSNode, Name.NamespaceName));
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            //todo: check if this is of any use and if it works (we need an element inside another that requires the same namespace)
                            CSHTML5.Interop.ExecuteJavaScriptAsync("$0.setAttribute($1, $2)", INTERNAL_containerJSNode, Name.LocalName, value);
                            object jsAttribute = CSHTML5.Interop.ExecuteJavaScript("$0.getAttribute($1).prefix = $2", INTERNAL_containerJSNode, Name.LocalName, str);
                        }
                        //note: we could have used the XElement.GetPrefixOfNamespace but we need the XElement for that so I went for a direct js approach

                        //todo: why is this Async? (if it was not Async we could probably restrain from using isJsValueUpToDate
                        CSHTML5.Interop.ExecuteJavaScriptAsync("$0.setAttributeNS($1, $2, $3)", INTERNAL_containerJSNode, Name.NamespaceName, Name.LocalName, value);
                    }
                    isJsValueUpToDate = true;
                }

                //object attributejs = CSHTML5.Interop.ExecuteJavaScript("$0.getAttributeNS($0,$1)", INTERNAL_containerJSNode, Name.NamespaceName, Name.LocalName);
                //if (Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0 != undefined && $0 != null", attributejs)))
                //{
                //    _value = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.value = $1", attributejs, value));
                //}
                _value = value;
            }
        }

#region explicit operators

        //Note: DateTimeOffset's and decimal's explicit operators are not implemented because:
        //          - users cannot make new DateTimeOffset()
        //          - decimal.Parse is not implemented

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.Boolean value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.Boolean.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.Boolean.</param>
        /// <returns>A System.Boolean that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator bool(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.Boolean value.");
            }
            else
            {
                return bool.Parse(attribute.Value);
            }
        }
        public static explicit operator bool?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return bool.Parse(attribute.Value);
            }
        }

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.Int32 value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to an System.Int32.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.Int32.</param>
        /// <returns>A System.Int32 that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator int(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.int value.");
            }
            else
            {
                return int.Parse(attribute.Value);
            }
        }
        public static explicit operator int?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return int.Parse(attribute.Value);
            }
        }


        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.UInt32 value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.UInt32.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.UInt32.</param>
        /// <returns>A System.UInt32 that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator uint(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.uint value.");
            }
            else
            {
                return uint.Parse(attribute.Value);
            }
        }
        public static explicit operator uint?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return uint.Parse(attribute.Value);
            }
        }

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.Int64 value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to an System.Int64.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.Int64.</param>
        /// <returns>A System.Int64 that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator long(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.long value.");
            }
            else
            {
                return long.Parse(attribute.Value);
            }
        }
        public static explicit operator long?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return long.Parse(attribute.Value);
            }
        }

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.UInt64 value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.UInt64.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.UInt64.</param>
        /// <returns>A System.UInt64 that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator ulong(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.ulong value.");
            }
            else
            {
                return ulong.Parse(attribute.Value);
            }
        }
        public static explicit operator ulong?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return ulong.Parse(attribute.Value);
            }
        }

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.DateTime value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.DateTime.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.DateTime.</param>
        /// <returns>A System.DateTime that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator DateTime(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.DateTime value.");
            }
            else
            {
                return DateTime.Parse(attribute.Value);
            }
        }
        public static explicit operator DateTime?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return DateTime.Parse(attribute.Value);
            }
        }

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.Guid value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.Guid.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.Guid.</param>
        /// <returns>A System.Guid that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator Guid(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.Guid value.");
            }
            else
            {
                return Guid.Parse(attribute.Value);
            }
        }
        public static explicit operator Guid?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return Guid.Parse(attribute.Value);
            }
        }

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.TimeSpan value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.TimeSpan.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.TimeSpan.</param>
        /// <returns>A System.TimeSpan that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator TimeSpan(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.TimeSpan value.");
            }
            else
            {
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.TimeSpanParse(attribute.Value);
#else
                return TimeSpan.Parse(attribute.Value);
#endif
            }
        }
        public static explicit operator TimeSpan?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.TimeSpanParse(attribute.Value);
#else
                return TimeSpan.Parse(attribute.Value);
#endif
            }
        }

        //DateTimeOffset:
        //// Exceptions:
        ////   System.FormatException:
        ////     The attribute does not contain a valid System.DateTimeOffset value.
        ////
        ////   System.ArgumentNullException:
        ////     The attribute parameter is null.
        ///// <summary>
        ///// Cast the value of this System.Xml.Linq.XAttribute to a System.DateTimeOffset.
        ///// </summary>
        ///// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.DateTimeOffset.</param>
        ///// <returns>A System.DateTimeOffset that contains the content of this System.Xml.Linq.XAttribute.</returns>
        //public static explicit operator DateTimeOffset(XAttribute attribute)
        //{
        //    if (attribute == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null.");
        //    }
        //    if (string.IsNullOrWhiteSpace(attribute.Value))
        //    {
        //        throw new FormatException("The attribute does not contain a valid System.DateTimeOffset value.");
        //    }
        //    else
        //    {
        //        return DateTimeOffset.Parse(attribute.Value);
        //    }
        //}
        //public static explicit operator DateTimeOffset?(XAttribute attribute)
        //{
        //    if (attribute == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null.");
        //    }
        //    if (string.IsNullOrWhiteSpace(attribute.Value))
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return DateTimeOffset.Parse(attribute.Value);
        //    }
        //}

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.String.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.String.</param>
        /// <returns>A System.String that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator string(XAttribute attribute)
        {
            return attribute.Value;
        }

        //decimal:
        //// Exceptions:
        ////   System.FormatException:
        ////     The attribute does not contain a valid System.Decimal value.
        ////
        ////   System.ArgumentNullException:
        ////     The attribute parameter is null.
        ///// <summary>
        ///// Cast the value of this System.Xml.Linq.XAttribute to a System.Decimal.
        ///// </summary>
        ///// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.Decimal.</param>
        ///// <returns>A System.Decimal that contains the content of this System.Xml.Linq.XAttribute.</returns>
        //public static explicit operator decimal(XAttribute attribute)
        //{
        //    if (attribute == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null.");
        //    }
        //    if (string.IsNullOrWhiteSpace(attribute.Value))
        //    {
        //        throw new FormatException("The attribute does not contain a valid System.decimal value.");
        //    }
        //    else
        //    {
        //        return decimal.Parse(attribute.Value);
        //    }
        //}
        //public static explicit operator decimal?(XAttribute attribute)
        //{
        //    if (attribute == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null.");
        //    }
        //    if (string.IsNullOrWhiteSpace(attribute.Value))
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return decimal.Parse(attribute.Value);
        //    }
        //}

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.Double value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.Double.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.Double.</param>
        /// <returns>A System.Double that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator double(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.double value.");
            }
            else
            {
                return double.Parse(attribute.Value);
            }
        }
        public static explicit operator double?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return double.Parse(attribute.Value);
            }
        }

        // Exceptions:
        //   System.FormatException:
        //     The attribute does not contain a valid System.Single value.
        //
        //   System.ArgumentNullException:
        //     The attribute parameter is null.
        /// <summary>
        /// Cast the value of this System.Xml.Linq.XAttribute to a System.Single.
        /// </summary>
        /// <param name="attribute">The System.Xml.Linq.XAttribute to cast to System.Single.</param>
        /// <returns>A System.Single that contains the content of this System.Xml.Linq.XAttribute.</returns>
        public static explicit operator float(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                throw new FormatException("The attribute does not contain a valid System.float value.");
            }
            else
            {
                return float.Parse(attribute.Value);
            }
        }
        public static explicit operator float?(XAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("The parameter cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(attribute.Value))
            {
                return null;
            }
            else
            {
                return float.Parse(attribute.Value);
            }
        }

#endregion

        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The parent element is null.
        ///// <summary>
        ///// Removes this attribute from its parent element.
        ///// </summary>
        //public void Remove();

        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The value parameter is null.
        ////
        ////   System.ArgumentException:
        ////     The value is an System.Xml.Linq.XObject.
        ///// <summary>
        ///// Sets the value of this attribute.
        ///// </summary>
        ///// <param name="value">The value to assign to this attribute.</param>
        //public void SetValue(object value);

        ///// <summary>
        ///// Converts the current System.Xml.Linq.XAttribute object to a string representation.
        ///// </summary>
        ///// <returns>
        ///// A System.String containing the XML text representation of an attribute and
        ///// its value.
        ///// </returns>
        //public override string ToString();

#region non implemented

        ///// <summary>
        ///// Gets an empty collection of attributes.
        ///// </summary>
        //public static IEnumerable<XAttribute> EmptySequence { get; }

        ///// <summary>
        ///// Determines if this attribute is a namespace declaration.
        ///// </summary>
        //public bool IsNamespaceDeclaration { get; }

#endregion
    }
}
#endif