
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
