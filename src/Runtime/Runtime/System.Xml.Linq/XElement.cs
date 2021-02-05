

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


using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !CSHTML5NETSTANDARD
// already defined in .NET Standard
namespace System.Xml.Linq
{
    /// <summary>
    /// Represents an XML element.
    /// </summary>
    public partial class XElement : XContainer
    {

        internal XElement(object jsNode)
        {
            INTERNAL_jsnode = jsNode; 
            XName xName = new XName();
            //todo: add the namespaceName.
            if (CSHTML5.Interop.IsRunningInTheSimulator)
            {
                // Hack to improve the Simulator performance by making only one interop call rather than two:
                string concatenated = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.localName + '|' + $0.namespaceURI", INTERNAL_jsnode));
                int sepIndex = concatenated.IndexOf('|');
                xName.LocalName = concatenated.Substring(0, sepIndex);
                xName.Namespace = concatenated.Substring(sepIndex + 1);
            }
            else
            {
                xName.LocalName = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.localName", INTERNAL_jsnode));
                xName.Namespace = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.namespaceURI", INTERNAL_jsnode));
            }
            _name = xName;
        }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XElement class with the
        /// specified name.
        /// </summary>
        /// <param name="name">An System.Xml.Linq.XName that contains the name of the element.</param>
        public XElement(XName name)
        {
            _name = name;

            INTERNAL_jsnode = CreateJsNodeForName(name);
        }

        private object CreateJsNodeForName(XName name)
        {
            //todo: remove this from here, add a method called "addXNodeToJSTree" that uses the xmlDocument created by the XDocument to create the XNode and that adds it to its parent.
            object tempDoc = CSHTML5.Interop.ExecuteJavaScript(@"document.implementation.createDocument(null, ""temp"", null)");
            object jsNode;
            if (name.Namespace == null)
            {
                jsNode = CSHTML5.Interop.ExecuteJavaScript(@"$0.createElement($1)", tempDoc, name.LocalName); //todo: check if we should use the whole name (including the NamespaceName), probably using createElementNS in js.
            }
            else
            {
                jsNode = CSHTML5.Interop.ExecuteJavaScript(@"$0.createElementNS($1, $2)", tempDoc, name.NamespaceName, name.LocalName); //todo: check if we should use the whole name (including the NamespaceName), probably using createElementNS in js.
            }
            return jsNode;
        }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XElement class from another
        /// System.Xml.Linq.XElement object.
        /// </summary>
        /// <param name="other">An System.Xml.Linq.XElement object to copy from.</param>
        public XElement(XElement other) : this(other._name) { }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XElement class with the
        /// specified name and content.
        /// </summary>
        /// <param name="name">A System.Xml.Linq.XName that contains the element name.</param>
        /// <param name="content">The contents of the element.</param>
        public XElement(XName name, object content)
            : this(name)
        {
            if (content is IEnumerable)
            {
                IEnumerable contentAsIEnumerable = (IEnumerable)content;
                foreach (object current in contentAsIEnumerable)
                {
                    this.Add(current);
                }
            }
            else
            {
                this.Add(content);
            }
        }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XElement class with the
        /// specified name and content.
        /// </summary>
        /// <param name="name">A System.Xml.Linq.XName that contains the element name.</param>
        /// <param name="content">The initial content of the element.</param>
        public XElement(XName name, params object[] content)
            : this(name)
        {
            foreach (object contentElement in content)
            {
                this.Add(contentElement);
            }
        }

        XName _name; //todo-perf: set _name only when we access Name so that it is not systematically created. Ths is probably minor on the performaces so I left it that way for now.
        /// <summary>
        /// Gets or sets the name of this element.
        /// </summary>
        public XName Name
        {
            get
            {
                return _name;
                //XName xName = new XName();
                //xName.LocalName = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.localName", INTERNAL_jsnode));
                //return xName;
            }
            set
            {
                _name = value;

                //for public version:
                //create a new node with the new name:
                var newNode = CreateJsNodeForName(value);

                List<XAttribute> attributes = new List<XAttribute>();
                //copy the attributes into the new node:
                foreach (XAttribute attribute in Attributes())
                {
                    attributes.Add(attribute);
                }
                foreach (XAttribute attribute in attributes)
                {
                    MoveNodeElement(attribute, newNode);
                }

                List<XNode> nodes = new List<XNode>();
                //copy the children into the new node:
                foreach (XNode child in Nodes())
                {
                    nodes.Add(child);
                }
                foreach (XNode child in nodes)
                {
                    MoveNodeElement(child, newNode);
                }

                //insert the new node
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.insertAdjacentElement(""beforeBegin"", $1)", INTERNAL_jsnode, newNode); //todo: check if we should use the whole name (including the NamespaceName), probably using createElementNS in js.


                //remove the ancient node:
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.remove()", INTERNAL_jsnode); //todo: check if we should use the whole name (including the NamespaceName), probably using createElementNS in js.

                //remember the newNode as the node of this element:
                INTERNAL_jsnode = newNode;

            }
        }

        void MoveNodeElement(object nodeElement, object destinationNode)
        {
            if (nodeElement is XNode)
            {
                //we'll consider that it is a XNode for now:
                XNode contentAsXNode = ((XNode)nodeElement);
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.appendChild($1)", destinationNode, contentAsXNode.INTERNAL_jsnode);
            }
            else if (nodeElement is XAttribute)
            {
                XAttribute contentAsXAttribute = (XAttribute)nodeElement;
                contentAsXAttribute.INTERNAL_containerJSNode = destinationNode;

                if (string.IsNullOrWhiteSpace(contentAsXAttribute.Name.NamespaceName))
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.setAttribute($1, $2)", destinationNode, contentAsXAttribute.Name.LocalName, contentAsXAttribute.Value);
                }
                else
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.setAttributeNS($1, $2, $3)", destinationNode, contentAsXAttribute.Name.NamespaceName, contentAsXAttribute.Name.LocalName, contentAsXAttribute.Value);
                }
            }
        }

        /// <summary>
        /// Returns a collection of attributes of this element.
        /// </summary>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable`1 of System.Xml.Linq.XAttribute
        /// of attributes of this element.
        /// </returns>
        public IEnumerable<XAttribute> Attributes()
        {
            //get the attributes from the js then yield return them
            object jsAttributes = CSHTML5.Interop.ExecuteJavaScript("$0.attributes", INTERNAL_jsnode);
            int i = 0;
            while (true)
            {
                object currentJsAttribute = CSHTML5.Interop.ExecuteJavaScript("$0[$1]", jsAttributes, i);
                if ((CSHTML5.Interop.IsRunningInTheSimulator && (CSHTML5.Interop.IsUndefined(currentJsAttribute) || CSHTML5.Interop.IsNull(currentJsAttribute))) // Performance optimization when running in the Simulator by reducing the number of Interop calls
                    || Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0 == undefined || $0 == null", currentJsAttribute)))
                {
                    yield break;
                }
                else
                {
                    yield return GetAttributeFromJSAttribute(currentJsAttribute);
                }
                ++i;
            }
        }

        /// <summary>
        /// Returns a filtered collection of attributes of this element. Only elements
        /// that have a matching System.Xml.Linq.XName are included in the collection.
        /// </summary>
        /// <param name="name">The System.Xml.Linq.XName to match.</param>
        /// <returns>
        /// An System.Collections.Generic.IEnumerable`1 of System.Xml.Linq.XAttribute
        /// that contains the attributes of this element. Only elements that have a matching
        /// System.Xml.Linq.XName are included in the collection.
        /// </returns>
        public IEnumerable<XAttribute> Attributes(XName name)
        {
            //same as the previous one but while filtering the results (see if we can directly filter in the js).
            //OK, I don't know how we could have multiple results here and the js only provides a method to get a single element with the given name so we'll only return one element.
            object jsAttribute;
            if (!string.IsNullOrWhiteSpace(name.NamespaceName))
            {
                jsAttribute = CSHTML5.Interop.ExecuteJavaScript("$0.attributes.getNamedItemNS($1,$2)", INTERNAL_jsnode, name.NamespaceName, name.LocalName);
            }
            else
            {
                jsAttribute = CSHTML5.Interop.ExecuteJavaScript("$0.attributes.getNamedItem($1)", INTERNAL_jsnode, name.LocalName);
            }
            if (Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript("$0 != null && $0 != undefined", jsAttribute)))
            {
                yield return GetAttributeFromJSAttribute(jsAttribute);
            }
            yield break;
        }

        private static XAttribute GetAttributeFromJSAttribute(object jsAttribute)
        {
            XNamespace ns;
            XName nameWithPrefix;
            XName name;
            string value;
            string concatenated = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.namespaceURI + '|' + $0.name + '|' + $0.localName + '|' + $0.value", jsAttribute));

            string[] attributeDatas = concatenated.Split('|');
            if (attributeDatas.Length == 4)
            {
                ns = XNamespace.Get(attributeDatas[0]);
                nameWithPrefix = ns.GetName(attributeDatas[1]);
                name = ns.GetName(attributeDatas[2]);
                value = attributeDatas[3];
            }
            else
            {
                ns = XNamespace.Get(Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.namespaceURI", jsAttribute)));
                nameWithPrefix = ns.GetName(Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.name", jsAttribute)));
                name = ns.GetName(Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.localName", jsAttribute)));
                value = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.value", jsAttribute));
            }

            return new XAttribute(name, value, nameWithPrefix);
        }


        /// <summary>
        /// Gets the default System.Xml.Linq.XNamespace of this System.Xml.Linq.XElement.
        /// </summary>
        /// <returns>
        /// A System.Xml.Linq.XNamespace that contains the default namespace of this
        /// System.Xml.Linq.XElement.
        /// </returns>
        public XNamespace GetDefaultNamespace()
        {
            string str = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.namespaceURI", INTERNAL_jsnode));
            return XNamespace.Get(str);
        }

        /// <summary>
        /// Gets the namespace associated with a particular prefix for this System.Xml.Linq.XElement.
        /// </summary>
        /// <param name="prefix">A string that contains the namespace prefix to look up.</param>
        /// <returns>
        /// An System.Xml.Linq.XNamespace for the namespace associated with the prefix
        /// for this System.Xml.Linq.XElement.
        /// </returns>
        public XNamespace GetNamespaceOfPrefix(string prefix)
        {
            string str = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.lookupNamespaceURI($1)", INTERNAL_jsnode, prefix));
            return XNamespace.Get(str);
        }

        /// <summary>
        /// Gets the prefix associated with a namespace for this System.Xml.Linq.XElement.
        /// </summary>
        /// <param name="ns">An System.Xml.Linq.XNamespace to look up.</param>
        /// <returns>A System.String that contains the namespace prefix.</returns>
        public string GetPrefixOfNamespace(XNamespace ns)
        {
            string str = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.lookupPrefix($1)", INTERNAL_jsnode, ns.NamespaceName));
            return str;
        }

        // Exceptions:
        //   System.ArgumentException:
        //     The value is an instance of System.Xml.Linq.XObject.
        /// <summary>
        /// Sets the value of an attribute, adds an attribute, or removes an attribute.
        /// </summary>
        /// <param name="name">An System.Xml.Linq.XName that contains the name of the attribute to change.</param>
        /// <param name="value">
        /// The value to assign to the attribute. The attribute is removed if the value
        /// is null. Otherwise, the value is converted to its string representation and
        /// assigned to the System.Xml.Linq.XAttribute.Value property of the attribute.
        /// </param>
        public void SetAttributeValue(XName name, object value)
        {
            if (value != null)
            {
                //Note: the Add method calls setAttribute in js, which already handles the case where the attribute already exists.
                this.Add(new XAttribute(name, value));
            }
            else
            {
                //if the value is null, it means we want to remove the attribute:
                if (string.IsNullOrWhiteSpace(name.NamespaceName))
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.removeAttributeNS($1)", INTERNAL_jsnode, name.LocalName);
                }
                else
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("$0.removeAttributeNS($1,$2)", INTERNAL_jsnode, name.NamespaceName, name.LocalName);
                }
            }
        }

        // Returns:
        //     A System.String that contains all of the text content of this element. If
        //     there are multiple text nodes, they will be concatenated.
        /// <summary>
        /// Gets or sets the concatenated text contents of this element.
        /// </summary>
        public string Value
        {
            get
            {
                return Convert.ToString(CSHTML5.Interop.ExecuteJavaScript(@"$0.textContent", INTERNAL_jsnode));
            }
            set
            {
                CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.textContent = $1", INTERNAL_jsnode, value);
            }
        }


        #region explicit operators

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.Guid.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.Guid.</param>
        /// <returns>A System.Guid that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator Guid(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string guidAsString = ((XText)content).Value;
                return Guid.Parse(guidAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to Guid.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.Double.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.Double.</param>
        /// <returns>A System.Double that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator double(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string doubleAsString = ((XText)content).Value;
                return Double.Parse(doubleAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to Double.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.DateTime.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.DateTime.</param>
        /// <returns>A System.DateTime that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator DateTime(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string dateTimeAsString = ((XText)content).Value;
                return DateTime.Parse(dateTimeAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to DateTime.");
        }

        //todo: DateTimeOffset commented because when trying to test it, we did not allow the users to use the DateTimeOffset constructor 
        //      decimal commented because in Parse causes an exception in the browser.

        //// Exceptions:
        ////   System.FormatException:
        ////     The element does not contain a valid System.Decimal value.
        ////
        ////   System.ArgumentNullException:
        ////     The element parameter is null.
        ///// <summary>
        ///// Cast the value of this System.Xml.Linq.XElement to a System.Decimal.
        ///// </summary>
        ///// <param name="element">The System.Xml.Linq.XElement to cast to System.Decimal.</param>
        ///// <returns>A System.Decimal that contains the content of this System.Xml.Linq.XElement.</returns>
        //public static explicit operator decimal(XElement element)
        //{
        //    if (element == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null");
        //    }
        //    XNode content = element.Nodes().First();
        //    if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
        //    {
        //        string decimalAsString = ((XText)content).Value;
        //        return decimal.Parse(decimalAsString); //this throws a FormatException when it doesn't work I think.
        //    }
        //    throw new FormatException("Could not convert element to decimal.");
        //}

        //// Exceptions:
        ////   System.FormatException:
        ////     The element does not contain a valid System.DateTimeOffset value.
        ////
        ////   System.ArgumentNullException:
        ////     The element parameter is null.
        ///// <summary>
        ///// Cast the value of this System.Xml.Linq.XAttribute to a System.DateTimeOffset.
        ///// </summary>
        ///// <param name="element">The System.Xml.Linq.XElement to cast to System.DateTimeOffset.</param>
        ///// <returns>A System.DateTimeOffset that contains the content of this System.Xml.Linq.XElement.</returns>
        //public static explicit operator DateTimeOffset(XElement element)
        //{
        //    if (element == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null");
        //    }
        //    XNode content = element.Nodes().First();
        //    if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
        //    {
        //        string dateTimeOffsetAsString = ((XText)content).Value;
        //        return DateTimeOffset.Parse(dateTimeOffsetAsString); //this throws a FormatException when it doesn't work I think.
        //    }
        //    throw new FormatException("Could not convert element to DateTimeOffset.");
        //}


        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to an System.Int64.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.Int64.</param>
        /// <returns>A System.Int64 that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator long(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string longAsString = ((XText)content).Value;
                return long.Parse(longAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to long.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to an System.Int32.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.Int32.</param>
        /// <returns>A System.Int32 that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator int(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string intAsString = ((XText)content).Value;
                return int.Parse(intAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to int.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.Boolean.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.Boolean.</param>
        /// <returns>A System.Boolean that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator bool(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string boolAsString = ((XText)content).Value;
                return bool.Parse(boolAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to bool.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.UInt64.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.UInt64.</param>
        /// <returns>A System.UInt64 that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator ulong(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string ulongAsString = ((XText)content).Value;
                return ulong.Parse(ulongAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to UInt64.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.UInt32.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.UInt32.</param>
        /// <returns>A System.UInt32 that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator uint(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string uintAsString = ((XText)content).Value;
                return uint.Parse(uintAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to UInt32.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.TimeSpan.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.TimeSpan.</param>
        /// <returns>A System.TimeSpan that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator TimeSpan(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string timeSpanAsString = ((XText)content).Value;
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.TimeSpanParse(timeSpanAsString);
#else
                return TimeSpan.Parse(timeSpanAsString);
#endif
            }
            throw new FormatException("Could not convert element to TimeSpan.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.String.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.String.</param>
        /// <returns>A System.String that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator string(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                return ((XText)content).Value;
            }
            throw new FormatException("Could not convert element to UInt32.");
        }

        /// <summary>
        /// Cast the value of this System.Xml.Linq.XElement to a System.Single.
        /// </summary>
        /// <param name="element">The System.Xml.Linq.XElement to cast to System.Single.</param>
        /// <returns>A System.Single that contains the content of this System.Xml.Linq.XElement.</returns>
        public static explicit operator float(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = element.Nodes().First();
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string floatAsString = ((XText)content).Value;
                return float.Parse(floatAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to float.");
        }

        public static explicit operator bool? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string boolAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(boolAsString))
                {
                    return null;
                }
                return bool.Parse(boolAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to bool.");
        }
        public static explicit operator DateTime? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string dateTimeAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(dateTimeAsString))
                {
                    return null;
                }
                return DateTime.Parse(dateTimeAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to Datetime.");
        }
        //public static explicit operator DateTimeOffset?(XElement element)
        //{
        //    if (element == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null");
        //    }
        //    XNode content = null;
        //    foreach (XNode node in element.Nodes())
        //    {
        //        content = node;
        //        break;
        //    }
        //    if (content == null)
        //    {
        //        return null;
        //    }
        //    if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
        //    {
        //        string dateTimeOffsetAsString = ((XText)content).Value;
        //        if (string.IsNullOrWhiteSpace(dateTimeOffsetAsString))
        //        {
        //            return null;
        //        }
        //        return DateTimeOffset.Parse(dateTimeOffsetAsString); //this throws a FormatException when it doesn't work I think.
        //    }
        //    throw new FormatException("Could not convert element to DateTimeOffset.");
        //}
        //public static explicit operator decimal?(XElement element)
        //{
        //    if (element == null)
        //    {
        //        throw new ArgumentNullException("The parameter cannot be null");
        //    }
        //    XNode content = null;
        //    foreach (XNode node in element.Nodes())
        //    {
        //        content = node;
        //        break;
        //    }
        //    if (content == null)
        //    {
        //        return null;
        //    }
        //    if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
        //    {
        //        string decimalAsString = ((XText)content).Value;
        //        if (string.IsNullOrWhiteSpace(decimalAsString))
        //        {
        //            return null;
        //        }
        //        return decimal.Parse(decimalAsString); //this throws a FormatException when it doesn't work I think.
        //    }
        //    throw new FormatException("Could not convert element to decimal.");
        //}
        public static explicit operator double? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string doubleAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(doubleAsString))
                {
                    return null;
                }
                return double.Parse(doubleAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to double.");
        }
        public static explicit operator Guid? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string guidAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(guidAsString))
                {
                    return null;
                }
                return Guid.Parse(guidAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to Guid.");
        }
        public static explicit operator int? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string intAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(intAsString))
                {
                    return null;
                }
                return int.Parse(intAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to int.");
        }
        public static explicit operator long? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string longAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(longAsString))
                {
                    return null;
                }
                return long.Parse(longAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to long.");
        }
        public static explicit operator float? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string floatAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(floatAsString))
                {
                    return null;
                }
                return float.Parse(floatAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to float.");
        }
        public static explicit operator TimeSpan? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string timeSpanAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(timeSpanAsString))
                {
                    return null;
                }
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.TimeSpanParse(timeSpanAsString);
#else
                return TimeSpan.Parse(timeSpanAsString);
#endif

            }
            throw new FormatException("Could not convert element to TimeSpan.");
        }
        public static explicit operator uint? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string uintAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(uintAsString))
                {
                    return null;
                }
                return uint.Parse(uintAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to uint.");
        }
        public static explicit operator ulong? (XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("The parameter cannot be null");
            }
            XNode content = null;
            foreach (XNode node in element.Nodes())
            {
                content = node;
                break;
            }
            if (content == null)
            {
                return null;
            }
            if (content is XText) //I'm assuming the sole content of the element must be a XText that contains the Guid
            {
                string ulongAsString = ((XText)content).Value;
                if (string.IsNullOrWhiteSpace(ulongAsString))
                {
                    return null;
                }
                return ulong.Parse(ulongAsString); //this throws a FormatException when it doesn't work I think.
            }
            throw new FormatException("Could not convert element to ulong.");
        }

        #endregion

        internal override XNode Clone()
        {
            XElement clone = new XElement(Name, Nodes());
            foreach (XAttribute attribute in Attributes())
            {
                clone.Add(attribute);
            }
            return clone;
        }

        #region not implemented

        ///// <summary>
        ///// Initializes a new instance of the System.Xml.Linq.XElement class from an
        ///// System.Xml.Linq.XStreamingElement object.
        ///// </summary>
        ///// <param name="other">
        ///// A System.Xml.Linq.XStreamingElement that contains unevaluated queries that
        ///// will be iterated for the contents of this System.Xml.Linq.XElement.
        ///// </param>
        //public XElement(XStreamingElement other);

        ///// <summary>
        ///// Gets an empty collection of elements.
        ///// </summary>
        //public static IEnumerable<XElement> EmptySequence { get; }

        ///// <summary>
        ///// Gets the first attribute of this element.
        ///// </summary>
        //public XAttribute FirstAttribute { get; }

        ///// <summary>
        ///// Gets a value indicating whether this element as at least one attribute.
        ///// </summary>
        //public bool HasAttributes { get; }

        ///// <summary>
        ///// Gets a value indicating whether this element has at least one child element.
        ///// </summary>
        //public bool HasElements { get; }

        ///// <summary>
        ///// Gets a value indicating whether this element contains no content.
        ///// </summary>
        //public bool IsEmpty { get; }

        ///// <summary>
        ///// Gets the last attribute of this element.
        ///// </summary>
        //public XAttribute LastAttribute { get; }

        //// Returns:
        ////     The node type. For System.Xml.Linq.XElement objects, this value is System.Xml.XmlNodeType.Element.
        ///// <summary>
        ///// Gets the node type for this node.
        ///// </summary>
        //public override XmlNodeType NodeType { get; }

        ///// <summary>
        ///// Returns a collection of elements that contain this element, and the ancestors
        ///// of this element.
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<XElement> AncestorsAndSelf();

        ///// <summary>
        ///// Returns a filtered collection of elements that contain this element, and
        ///// the ancestors of this element. Only elements that have a matching System.Xml.Linq.XName
        ///// are included in the collection.
        ///// </summary>
        ///// <param name="name">The System.Xml.Linq.XName to match.</param>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// that contain this element, and the ancestors of this element. Only elements
        ///// that have a matching System.Xml.Linq.XName are included in the collection.
        ///// </returns>
        //public IEnumerable<XElement> AncestorsAndSelf(XName name);

        ///// <summary>
        ///// Returns the System.Xml.Linq.XAttribute of this System.Xml.Linq.XElement that
        ///// has the specified System.Xml.Linq.XName.
        ///// </summary>
        ///// <param name="name">The System.Xml.Linq.XName of the System.Xml.Linq.XAttribute to get.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XAttribute that has the specified System.Xml.Linq.XName;
        ///// null if there is no attribute with the specified name.
        ///// </returns>
        //public XAttribute Attribute(XName name);

        ///// <summary>
        ///// Returns a collection of nodes that contain this element, and all descendant
        ///// nodes of this element, in document order.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XNode that
        ///// contain this element, and all descendant nodes of this element, in document
        ///// order.
        ///// </returns>
        //public IEnumerable<XNode> DescendantNodesAndSelf();

        ///// <summary>
        ///// Returns a collection of elements that contain this element, and all descendant
        ///// elements of this element, in document order.
        ///// </summary>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// of elements that contain this element, and all descendant elements of this
        ///// element, in document order.
        ///// </returns>
        //public IEnumerable<XElement> DescendantsAndSelf();

        ///// <summary>
        ///// Returns a filtered collection of elements that contain this element, and
        ///// all descendant elements of this element, in document order. Only elements
        ///// that have a matching System.Xml.Linq.XName are included in the collection.
        ///// </summary>
        ///// <param name="name">The System.Xml.Linq.XName to match.</param>
        ///// <returns>
        ///// An System.Collections.Generic.IEnumerable<T> of System.Xml.Linq.XElement
        ///// that contain this element, and all descendant elements of this element, in
        ///// document order. Only elements that have a matching System.Xml.Linq.XName
        ///// are included in the collection.
        ///// </returns>
        //public IEnumerable<XElement> DescendantsAndSelf(XName name);


        ///// <summary>
        ///// Creates a new System.Xml.Linq.XElement instance by using the specified stream.
        ///// </summary>
        ///// <param name="stream">The stream that contains the XML data.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XElement object used to read the data that is contained
        ///// in the stream.
        ///// </returns>
        //public static XElement Load(Stream stream);

        ///// <summary>
        ///// Loads an System.Xml.Linq.XElement from a file.
        ///// </summary>
        ///// <param name="uri">A URI string referencing the file to load into a new System.Xml.Linq.XElement.</param>
        ///// <returns>A System.Xml.Linq.XElement that contains the contents of the specified file.</returns>
        //public static XElement Load(string uri);

        ///// <summary>
        ///// Loads an System.Xml.Linq.XElement from a System.IO.TextReader.
        ///// </summary>
        ///// <param name="textReader">
        ///// A System.IO.TextReader that will be read for the System.Xml.Linq.XElement
        ///// content.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XElement that contains the XML that was read from the
        ///// specified System.IO.TextReader.
        ///// </returns>
        //public static XElement Load(TextReader textReader);

        ///// <summary>
        ///// Loads an System.Xml.Linq.XElement from an System.Xml.XmlReader.
        ///// </summary>
        ///// <param name="reader">A System.Xml.XmlReader that will be read for the content of the System.Xml.Linq.XElement.</param>
        ///// <returns>
        ///// An System.Xml.Linq.XElement that contains the XML that was read from the
        ///// specified System.Xml.XmlReader.
        ///// </returns>
        //public static XElement Load(XmlReader reader);

        ///// <summary>
        ///// Creates a new System.Xml.Linq.XElement instance by using the specified stream,
        ///// optionally preserving white space, setting the base URI, and retaining line
        ///// information.
        ///// </summary>
        ///// <param name="stream">The stream containing the XML data.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions object that specifies whether to load base
        ///// URI and line information.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XElement object used to read the data that the stream
        ///// contains.
        ///// </returns>
        //public static XElement Load(Stream stream, LoadOptions options);

        ///// <summary>
        ///// Loads an System.Xml.Linq.XElement from a file, optionally preserving white
        ///// space, setting the base URI, and retaining line information.
        ///// </summary>
        ///// <param name="uri">A URI string referencing the file to load into an System.Xml.Linq.XElement.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies white space behavior, and whether
        ///// to load base URI and line information.
        ///// </param>
        ///// <returns>An System.Xml.Linq.XElement that contains the contents of the specified file.</returns>
        //public static XElement Load(string uri, LoadOptions options);

        ///// <summary>
        ///// Loads an System.Xml.Linq.XElement from a System.IO.TextReader, optionally
        ///// preserving white space and retaining line information.
        ///// </summary>
        ///// <param name="textReader">
        ///// A System.IO.TextReader that will be read for the System.Xml.Linq.XElement
        ///// content.
        ///// </param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies white space behavior, and whether
        ///// to load base URI and line information.
        ///// </param>
        ///// <returns>
        ///// A System.Xml.Linq.XElement that contains the XML that was read from the
        ///// specified System.IO.TextReader.
        ///// </returns>
        //public static XElement Load(TextReader textReader, LoadOptions options);

        ///// <summary>
        ///// Loads an System.Xml.Linq.XElement from an System.Xml.XmlReader, optionally
        ///// preserving white space, setting the base URI, and retaining line information.
        ///// </summary>
        ///// <param name="reader">A System.Xml.XmlReader that will be read for the content of the System.Xml.Linq.XElement.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies white space behavior, and whether
        ///// to load base URI and line information.
        ///// </param>
        ///// <returns>
        ///// An System.Xml.Linq.XElement that contains the XML that was read from the
        ///// specified System.Xml.XmlReader.
        ///// </returns>
        //public static XElement Load(XmlReader reader, LoadOptions options);

        ///// <summary>
        ///// Load an System.Xml.Linq.XElement from a string that contains XML.
        ///// </summary>
        ///// <param name="text">A System.String that contains XML.</param>
        ///// <returns>An System.Xml.Linq.XElement populated from the string that contains XML.</returns>
        //public static XElement Parse(string text);

        ///// <summary>
        ///// Load an System.Xml.Linq.XElement from a string that contains XML, optionally
        ///// preserving white space and retaining line information.
        ///// </summary>
        ///// <param name="text">A System.String that contains XML.</param>
        ///// <param name="options">
        ///// A System.Xml.Linq.LoadOptions that specifies white space behavior, and whether
        ///// to load base URI and line information.
        ///// </param>
        ///// <returns>An System.Xml.Linq.XElement populated from the string that contains XML.</returns>
        //public static XElement Parse(string text, LoadOptions options);

        ///// <summary>
        ///// Removes nodes and attributes from this System.Xml.Linq.XElement.
        ///// </summary>
        //public void RemoveAll();

        ///// <summary>
        ///// Removes the attributes of this System.Xml.Linq.XElement.
        ///// </summary>
        //public void RemoveAttributes();

        ///// <summary>
        ///// Replaces the child nodes and the attributes of this element with the specified
        ///// content.
        ///// </summary>
        ///// <param name="content">The content that will replace the child nodes and attributes of this element.</param>
        //public void ReplaceAll(object content);

        ///// <summary>
        ///// Replaces the child nodes and the attributes of this element with the specified
        ///// content.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects.</param>
        //public void ReplaceAll(params object[] content);

        ///// <summary>
        ///// Replaces the attributes of this element with the specified content.
        ///// </summary>
        ///// <param name="content">The content that will replace the attributes of this element.</param>
        //public void ReplaceAttributes(object content);

        ///// <summary>
        ///// Replaces the attributes of this element with the specified content.
        ///// </summary>
        ///// <param name="content">A parameter list of content objects.</param>
        //public void ReplaceAttributes(params object[] content);

        ///// <summary>
        ///// Outputs this System.Xml.Linq.XElement to the specified System.IO.Stream.
        ///// </summary>
        ///// <param name="stream">The stream to output this System.Xml.Linq.XElement to.</param>
        //public void Save(Stream stream);

        ///// <summary>
        ///// Serialize this element to a System.IO.TextWriter.
        ///// </summary>
        ///// <param name="textWriter">
        ///// A System.IO.TextWriter that the System.Xml.Linq.XElement will be written
        ///// to.
        ///// </param>
        //public void Save(TextWriter textWriter);

        ///// <summary>
        ///// Serialize this element to an System.Xml.XmlWriter.
        ///// </summary>
        ///// <param name="writer">
        ///// A System.Xml.XmlWriter that the System.Xml.Linq.XElement will be written
        ///// to.
        ///// </param>
        //public void Save(XmlWriter writer);

        ///// <summary>
        ///// Outputs this System.Xml.Linq.XElement to the specified System.IO.Stream,
        ///// optionally specifying formatting behavior.
        ///// </summary>
        ///// <param name="stream">The stream to output this System.Xml.Linq.XElement to.</param>
        ///// <param name="options">A System.Xml.Linq.SaveOptions object that specifies formatting behavior.</param>
        //public void Save(Stream stream, SaveOptions options);

        ///// <summary>
        ///// Serialize this element to a System.IO.TextWriter, optionally disabling formatting.
        ///// </summary>
        ///// <param name="textWriter">The System.IO.TextWriter to output the XML to.</param>
        ///// <param name="options">A System.Xml.Linq.SaveOptions that specifies formatting behavior.</param>
        //public void Save(TextWriter textWriter, SaveOptions options);

        //// Exceptions:
        ////   System.ArgumentException:
        ////     The value is an instance of System.Xml.Linq.XObject.
        ///// <summary>
        ///// Sets the value of a child element, adds a child element, or removes a child
        ///// element.
        ///// </summary>
        ///// <param name="name">An System.Xml.Linq.XName that contains the name of the child element to change.</param>
        ///// <param name="value">
        ///// The value to assign to the child element. The child element is removed if
        ///// the value is null. Otherwise, the value is converted to its string representation
        ///// and assigned to the System.Xml.Linq.XElement.Value property of the child
        ///// element.
        ///// </param>
        //public void SetElementValue(XName name, object value);

        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The value is null.
        ////
        ////   System.ArgumentException:
        ////     The value is an System.Xml.Linq.XObject.
        ///// <summary>
        ///// Sets the value of this element.
        ///// </summary>
        ///// <param name="value">
        ///// The value to assign to this element. The value is converted to its string
        ///// representation and assigned to the System.Xml.Linq.XElement.Value property.
        ///// </param>
        //public void SetValue(object value);

        ///// <summary>
        ///// Write this element to an System.Xml.XmlWriter.
        ///// </summary>
        ///// <param name="writer">An System.Xml.XmlWriter into which this method will write.</param>
        //public override void WriteTo(XmlWriter writer);
        #endregion
    }
}
#endif