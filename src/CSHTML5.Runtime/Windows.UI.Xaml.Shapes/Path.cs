

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
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Shapes
#else
namespace Windows.UI.Xaml.Shapes
#endif
{
    /// <summary>
    /// Draws a series of connected lines and curves. The line and curve dimensions
    /// are declared through the Data property, and can be specified either with
    /// a path-specific mini-language, or with an object model.
    /// </summary>
    public partial class Path : Shape
    {
        //internal dynamic canvasDomElement;

        //todo: Set the default Stretch to none and position to top left (if not already top left)
        // Stretch behaviour:
        // - None: no stretch, potential clipping when size explicitely defined
        // - others than None: size can only be smaller if container is explicitely smaller than it.
        //                     when container size not explicitely defined, takes the size closest to its non-Stretched size that allows it to fill its role.
        //                     example: in a vertical StackPanel with an element inside and a path:
        //                                                  - if element inside is wider than path, in the case of Stretch = Fill or UniformToFill, the path will take the width of the element
        //                                                  - if path is wider than element inside, the path will take the width of its non-stretched form
        //                                                  - its height will depend on if it is Fill, Uniform or UniformToFill:
        //                                                          -Fill: the height will be the one of its non-stretched form.
        //                                                          -Uniform: the height will be that of its non-stretched form and the shape will be centered inside the StackPanel. (In this case, filling its role means having the height bigger than the size of the minimum height of an element in the stackPanel, which is 0.
        //                                                          -UniformToFill: the height will be multiplied by the same factor as its width from its non-stretched form BUT the visible height will only be the size of the non-stretched form
        // STRETCH ROLES:
        // - Fill: Width = container's width; Height = container's height.
        // - Uniform: Width = Width*X; height = height * X, X so that either Width = container's width or Height = container's height, with Width <= container's width and Height <= container's height.
        // - UniformToFill: Width = Width*X; height = height * X, X so that either Width = container's width or Height = container's height, with Width >= container's width and Height >= container's height.

        //todo: find how to know if the container is adapting to the content's size in each direction OR find a way to make it work without knowing

        //idea (far from perfect because it implies to set the width of the canvas once, see the resule then re-set it.) : 
        //          - we put the canvas with the size of the non stretched path
        //          - then we apply the width and height of the div to the canvas, depending on the Stretch mode
        // Why? --> 3 possibilities when setting the size to the non stretched path:
        //                - the div is smaller than the path --> the container has a size limited to the remaining space
        //                - the div is bigger than the path --> the container was bigger than the path anyway so we can make the stretches as needed.
        //                - the div is the same size as the path --> the container either adapter to the path's size or was already supposed to have the same size.
        //NOTE: the above results must be observed on both directions "separately" then put together to know how to handle the stretch

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return INTERNAL_ShapesDrawHelpers.CreateDomElementForPathAndSimilar(this, parentRef, out _canvasDomElement, out domElementWhereToPlaceChildren);
        }
        /// <summary>
        /// Gets or sets a Geometry that specifies the shape to be drawn.
        /// </summary>
        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        /// <summary>
        /// Identifies the Data dependency property.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Geometry), typeof(Path), new PropertyMetadata(null, Data_Changed));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Geometry oldgeometry = (Geometry)e.OldValue;
            Geometry newgeometry = (Geometry)e.NewValue;
            Path path = (Path)d;
            if (e.NewValue != e.OldValue && path._isLoaded)
            {
                if (oldgeometry != null)
                {
                    oldgeometry.SetParentPath(null);
                }
                newgeometry.SetParentPath(path);
                path.ScheduleRedraw();
            }
        }

        internal void RefreshChildrenParent()
        {
            Data.SetParentPath(this);
        }


        //protected internal override void INTERNAL_OnAttachedToVisualTree()
        //{
        //    ScheduleRedraw();
        //}

        protected internal override void Redraw()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (Data != null)
                {
                    double minX = double.MaxValue;
                    double minY = double.MaxValue;
                    double maxX = double.MinValue;
                    double maxY = double.MinValue;
                    if (Data is PathGeometry)
                    {
                        ((PathGeometry)Data).UpdateStrokeThickness(StrokeThickness);
                    }
                    Data.GetMinMaxXY(ref minX, ref maxX, ref minY, ref maxY);

                    Size shapeActualSize;
                    INTERNAL_ShapesDrawHelpers.PrepareStretch(this, _canvasDomElement, minX, maxX, minY, maxY, Stretch, out shapeActualSize);
                    double horizontalMultiplicator;
                    double verticalMultiplicator;
                    double xOffsetToApplyBeforeMultiplication;
                    double yOffsetToApplyBeforeMultiplication;
                    double xOffsetToApplyAfterMultiplication;
                    double yOffsetToApplyAfterMultiplication;
                    double strokeThickness = StrokeThickness;
                    if (Stroke == null)
                    {
                        strokeThickness = 0;
                    }

                    INTERNAL_ShapesDrawHelpers.GetMultiplicatorsAndOffsetForStretch(this, strokeThickness, minX, maxX, minY, maxY, Stretch, shapeActualSize, out horizontalMultiplicator, out verticalMultiplicator, out xOffsetToApplyBeforeMultiplication, out yOffsetToApplyBeforeMultiplication, out xOffsetToApplyAfterMultiplication, out yOffsetToApplyAfterMultiplication, out _marginOffsets);

                    ApplyMarginToFixNegativeCoordinates(new Point());
                    if (Stretch == Stretch.None)
                    {
                        ApplyMarginToFixNegativeCoordinates(_marginOffsets);
                    }

                    // A call to "context.beginPath" is required on IE and Edge for the figures to be drawn properly (cf. ZenDesk #971):
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.getContext('2d').beginPath()", _canvasDomElement);

                    //problem here: the shape seems to be overall smaller than intended due to the edges of the path not being sharp?
                    Data.DefineInCanvas(this, _canvasDomElement, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, xOffsetToApplyAfterMultiplication, yOffsetToApplyAfterMultiplication, shapeActualSize); //puts the lines and stuff in the context
                    Shape.DrawFillAndStroke(this, Data.GetFillRuleAsString(), minX, minY, maxX, maxY, horizontalMultiplicator, verticalMultiplicator, xOffsetToApplyBeforeMultiplication, yOffsetToApplyBeforeMultiplication, shapeActualSize);
                }
            }
        }
    }
}