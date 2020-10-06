

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Shapes;
#else
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents the geometry of a line.
    /// </summary>
    public sealed partial class LineGeometry : Geometry
    {
        // <summary>
        // Initializes a new instance of the LineGeometry class that has no length.
        // </summary>
        public LineGeometry()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public LineGeometry(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        // Returns:
        //     The end point of the line. The default is a Point with value 0,0.
        /// <summary>
        /// Gets or sets the end point of a line.
        /// </summary>
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }
        /// <summary>
        /// Identifies the EndPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Point), typeof(LineGeometry), new PropertyMetadata(new Point(), EndPoint_Changed));

        private static void EndPoint_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineGeometry geometry = (LineGeometry)d;
            if (geometry.ParentPath != null)
            {
                geometry.ParentPath.ScheduleRedraw();
            }
        }

        // Returns:
        //     The start point of the line. The default is a Point with value 0,0.
        /// <summary>
        /// Gets or sets the start point of the line.
        /// </summary>
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }
        /// <summary>
        /// Identifies the StartPoint dependency property.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(LineGeometry), new PropertyMetadata(new Point(), StartPoint_Changed));

        private static void StartPoint_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LineGeometry geometry = (LineGeometry)d;
            if (geometry.ParentPath != null)
            {
                geometry.ParentPath.ScheduleRedraw();
            }
        }

        internal protected override void GetMinMaxXY(ref double minX, ref double maxX, ref double minY, ref double maxY)
        {
            double maxAbs = StartPoint.X > EndPoint.X ? StartPoint.X : EndPoint.X;
            double minAbs = StartPoint.X < EndPoint.X ? StartPoint.X : EndPoint.X;
            double minOrd = StartPoint.Y < EndPoint.Y ? StartPoint.Y : EndPoint.Y;
            double maxOrd = StartPoint.Y > EndPoint.Y ? StartPoint.Y : EndPoint.Y;
            if (maxX < maxAbs)
            {
                maxX = maxAbs;
            }
            if (maxY < maxOrd)
            {
                maxY = maxOrd;
            }
            if (minX > minAbs)
            {
                minX = minAbs;
            }
            if (minY > minOrd)
            {
                minY = minOrd;
            }
        }

        // note: we only define the line. Erasing the previous one (if any) and actually drawing the 
        // new one should be made directly by the container.
        internal protected override void DefineInCanvas(Path path, 
                                                        object canvasDomElement, 
                                                        double horizontalMultiplicator, 
                                                        double verticalMultiplicator, 
                                                        double xOffsetToApplyBeforeMultiplication, 
                                                        double yOffsetToApplyBeforeMultiplication, 
                                                        double xOffsetToApplyAfterMultiplication, 
                                                        double yOffsetToApplyAfterMultiplication, 
                                                        Size shapeActualSize)
        {
            dynamic ctx = INTERNAL_HtmlDomManager.Get2dCanvasContext(canvasDomElement);

            ctx.moveTo(StartPoint.X, StartPoint.Y);
            ctx.lineTo(EndPoint.X, EndPoint.Y);
        }
    }
}