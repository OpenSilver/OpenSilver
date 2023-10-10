
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

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a segment of a <see cref="PathFigure"/> object.
    /// </summary>
    public abstract class PathSegment : DependencyObject
    {
        private Geometry _parentGeometry;

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