﻿

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
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Implements a markup extension that describes the location of the binding source relative to the position of the binding target.
    /// </summary>
    [ContentProperty("Mode")]
    public partial class RelativeSource : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the RelativeSource class by using default relative source mode.
        /// </summary>
        public RelativeSource() { }

        /// <summary>
        /// Initializes a new instance of the RelativeSource class by using provided relative source mode.
        /// </summary>
        /// <param name="relativeSourceMode">The relative source mode</param>
        public RelativeSource(RelativeSourceMode relativeSourceMode)
        {
            _mode = relativeSourceMode;
        }


        static RelativeSource()
        {
            TypeFromStringConverters.RegisterConverter(typeof(RelativeSourceMode), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string relativeSourceAsString)
        {
            switch (relativeSourceAsString)
            {
                case "None":
                    return (new RelativeSource() { Mode = RelativeSourceMode.None });
                case "Self":
                    return (new RelativeSource() { Mode = RelativeSourceMode.Self });
                case "TemplatedParent":
                    return (new RelativeSource() { Mode = RelativeSourceMode.TemplatedParent });
                default:
                    throw new FormatException(relativeSourceAsString + " is not an eligible value for a RelativeSource");
            }
        }


        RelativeSourceMode _mode = RelativeSourceMode.None;
        /// <summary>
        /// Gets or sets a value that describes the location of the binding source relative to the position of the binding target.
        /// Returns a value of the enumeration.
        /// </summary>
        public RelativeSourceMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }


        private int _ancestorLevel = 1;
        /// <summary>
        /// Gets or sets the level of ancestor to look for, in System.Windows.Data.RelativeSourceMode.FindAncestor
        /// mode. Use 1 to indicate the one nearest to the binding target element.
        /// </summary>
        public int AncestorLevel
        {
            get { return _ancestorLevel; }
            set
            {
                if(value < 1)
                {
                    throw new ArgumentOutOfRangeException("The ancestor level cannot be less than one.");
                }
                _ancestorLevel = value;
            }
        }

        private Type _ancestorType = null;
        /// <summary>
        /// Gets or sets the type of ancestor to look for.
        /// </summary>
        public Type AncestorType
        {
            get { return _ancestorType; }
            set
            {
                if (_mode == RelativeSourceMode.None)
                {
                    _mode = RelativeSourceMode.FindAncestor;
                }
                _ancestorType = value;
            }
        }

#if !BRIDGE
        public override object ProvideValue(IServiceProvider serviceProvider)
#else
        public override object ProvideValue(ServiceProvider serviceProvider)
#endif
        {
            // Not implemented
            return null;
        }
    }
}