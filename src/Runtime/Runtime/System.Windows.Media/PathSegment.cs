
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

using System.Collections.Generic;
using OpenSilver.Internal;

namespace System.Windows.Media
{
    /// <summary>
    /// Represents a segment of a <see cref="PathFigure"/> object.
    /// </summary>
    public abstract class PathSegment : DependencyObject
    {
        private Geometry _parentGeometry;

        /// <summary>
        /// Identifies the <see cref="IsStroked"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsStrokedProperty =
            DependencyProperty.Register(nameof(IsStroked),
                typeof(bool),
                typeof(PathSegment),
                new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value that indicates whether the segment is stroked.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the segment is stroked when a Pen is used to render the segment;
        /// otherwise, <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        [OpenSilver.NotImplemented]
        public bool IsStroked
        {
            get => (bool)GetValue(IsStrokedProperty);
            set => SetValueInternal(IsStrokedProperty, BooleanBoxes.Box(value));
        }

        internal PathSegment() { }

        internal void SetParentGeometry(Geometry geometry) => _parentGeometry = geometry;

        internal static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathSegment)d).InvalidateParentGeometry();
        }

        internal void InvalidateParentGeometry() => _parentGeometry?.RaisePathChanged();

        internal virtual IEnumerable<string> ToDataStream(IFormatProvider formatProvider)
        {
            throw new NotSupportedException($"ToDataStream() not supported on {GetType().Name}");
        }
    }
}