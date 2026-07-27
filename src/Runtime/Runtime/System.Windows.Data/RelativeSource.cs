
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

using System.Windows.Markup;

namespace System.Windows.Data
{
    /// <summary>
    /// Implements a markup extension that describes the location of the binding source relative to the position of the binding target.
    /// </summary>
    [ContentProperty(nameof(Mode))]
    public class RelativeSource : MarkupExtension
    {
        private RelativeSourceMode _mode = RelativeSourceMode.None;
        private int _ancestorLevel = 1;
        private Type _ancestorType = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeSource"/> class.
        /// </summary>
        public RelativeSource() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeSource"/> class with an initial mode.
        /// </summary>
        /// <param name="relativeSourceMode">
        /// One of the <see cref="RelativeSourceMode"/> values.
        /// </param>
        public RelativeSource(RelativeSourceMode relativeSourceMode)
        {
            _mode = relativeSourceMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeSource"/> class with an initial mode and 
        /// additional tree-walking qualifiers for finding the desired relative source.
        /// </summary>
        /// <param name="mode">
        /// One of the <see cref="RelativeSourceMode"/> values. For this signature to be relevant, this 
        /// should be <see cref="RelativeSourceMode.FindAncestor"/>.
        /// </param>
        /// <param name="ancestorType">
        /// The <see cref="Type"/> of ancestor to look for.
        /// </param>
        /// <param name="ancestorLevel">
        /// The ordinal position of the desired ancestor among all ancestors of the given type.
        /// </param>
        public RelativeSource(RelativeSourceMode mode, Type ancestorType, int ancestorLevel)
        {
            _mode = mode;
            AncestorType = ancestorType;
            AncestorLevel = ancestorLevel;
        }

        /// <summary>
        /// Gets or sets a value that describes the location of the binding source relative to the position of the binding target.
        /// Returns a value of the enumeration.
        /// </summary>
        public RelativeSourceMode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        /// <summary>
        /// Gets or sets the level of ancestor to look for, in System.Windows.Data.RelativeSourceMode.FindAncestor
        /// mode. Use 1 to indicate the one nearest to the binding target element.
        /// </summary>
        public int AncestorLevel
        {
            get { return _ancestorLevel; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("The ancestor level cannot be less than one.");
                }
                _ancestorLevel = value;
            }
        }

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

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Not implemented
            return null;
        }
    }
}