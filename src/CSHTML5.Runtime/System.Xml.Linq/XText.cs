

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
    /// Represents a text node.
    /// </summary>
    public partial class XText : XNode
    {

        internal XText(object jsNode)
        {
            INTERNAL_jsnode = jsNode;
            _value = Convert.ToString(CSHTML5.Interop.ExecuteJavaScript("$0.wholeText", jsNode));
        }

        /// <summary>
        /// Initializes a new instance of the System.Xml.Linq.XText class.
        /// </summary>
        /// <param name="value"></param>
        public XText(string value)
        {
            INTERNAL_jsnode = CSHTML5.Interop.ExecuteJavaScript("document.createTextNode($0)", value);
            _value = value;
        }

        string _value;
        /// <summary>
        /// Gets or sets the value of this node.
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                CSHTML5.Interop.ExecuteJavaScript("$0.nodeValue = $1", INTERNAL_jsnode, value);
            }
        }

        internal override XNode Clone()
        {
            return new XText(Value);
        }

#region not implemented

        ///// <summary>
        ///// Initializes a new instance of the System.Xml.Linq.XText class from another
        ///// System.Xml.Linq.XText object.
        ///// </summary>
        ///// <param name="other">The System.Xml.Linq.XText node to copy from.</param>
        //public XText(XText other);

        //// Returns:
        ////     The node type. For System.Xml.Linq.XText objects, this value is System.Xml.XmlNodeType.Text.
        ///// <summary>
        ///// Gets the node type for this node.
        ///// </summary>
        //public override XmlNodeType NodeType { get; }



        ///// <summary>
        ///// Writes this node to an System.Xml.XmlWriter.
        ///// </summary>
        ///// <param name="writer"></param>
        //public override void WriteTo(XmlWriter writer);

#endregion
    }
}
#endif