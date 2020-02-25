
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
#if !MIGRATION
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a segment of a PathFigure object.
    /// </summary>
    public partial class PathSegment : DependencyObject
    {
        internal Path INTERNAL_parentPath = null;
        internal void SetParentPath(Path path)
        {
            INTERNAL_parentPath = path;
        }

        /// <summary>
        /// Defines the segment in the canvas, then returns the position of the last point of the segment.
        /// </summary>
        /// <param name="xOffsetToApplyBeforeMultiplication"></param>
        /// <param name="yOffsetToApplyBeforeMultiplication"></param>
        /// <param name="xOffsetToApplyAfterMultiplication"></param>
        /// <param name="yOffsetToApplyAfterMultiplication"></param>
        /// <param name="horizontalMultiplicator"></param>
        /// <param name="verticalMultiplicator"></param>
        /// <param name="canvasDomElement"></param>
        /// <param name="previousLastPOint"></param>
        /// <returns>The position of the last point of the segment (that will be the starting position for the next segment).</returns>
        internal virtual Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, double yOffsetToApplyBeforeMultiplication, double xOffsetToApplyAfterMultiplication, double yOffsetToApplyAfterMultiplication, double horizontalMultiplicator, double verticalMultiplicator, object canvasDomElement, Point previousLastPOint)
        {
            throw new NotImplementedException();
        }

        internal virtual Point GetMaxXY()
        {
            throw new NotImplementedException();
        }

        internal virtual Point GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY, Point startingPoint)
        {
            throw new NotImplementedException();
        }
    }
}