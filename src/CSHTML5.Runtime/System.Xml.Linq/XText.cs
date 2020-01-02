
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
    /// Represents a text node.
    /// </summary>
    public class XText : XNode
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