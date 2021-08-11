

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
    public abstract partial class PathSegment : DependencyObject
    {
        #region Constructor

        internal PathSegment()
        {

        }

        #endregion

        #region Internal API

        internal Path ParentPath { get; private set; }

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
        internal abstract Point DefineInCanvas(double xOffsetToApplyBeforeMultiplication, 
                                               double yOffsetToApplyBeforeMultiplication, 
                                               double xOffsetToApplyAfterMultiplication, 
                                               double yOffsetToApplyAfterMultiplication, 
                                               double horizontalMultiplicator, 
                                               double verticalMultiplicator, 
                                               object canvasDomElement, 
                                               Point previousLastPOint);

        internal abstract Point GetMinMaxXY(ref double minX,
                                            ref double maxX,
                                            ref double minY,
                                            ref double maxY,
                                            Point startingPoint);

        internal abstract Point GetMaxXY();

        internal virtual void SetParentPath(Path path)
        {
            ParentPath = path;
        }

        #endregion
    }
}