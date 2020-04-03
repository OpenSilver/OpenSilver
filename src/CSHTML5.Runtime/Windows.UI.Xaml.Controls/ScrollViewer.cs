

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a scrollable area that can contain other visible elements.
    /// </summary>
    public sealed partial class ScrollViewer : ContentControl
    {
        double _verticalOffset = 0;
        double _horizontalOffset = 0;
        /// <summary>
        /// Initializes a new instance of the ScrollViewer class.
        /// </summary>
        public ScrollViewer() : base() { }


        /*
        public bool BringIntoViewOnFocusChange { get; set; }
        public static DependencyProperty BringIntoViewOnFocusChangeProperty { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the horizontal ScrollBar is visible.
        //
        // Returns:
        //     A Visibility that indicates whether the horizontal scroll bar is visible.
        //     The default value is Hidden.
        public Visibility ComputedHorizontalScrollBarVisibility { get; }
        //
        // Summary:
        //     Identifies the ComputedHorizontalScrollBarVisibility dependency property.
        //
        // Returns:
        //     The identifier for the ComputedHorizontalScrollBarVisibility dependency property.
        public static DependencyProperty ComputedHorizontalScrollBarVisibilityProperty { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the vertical ScrollBar is visible.
        //
        // Returns:
        //     A Visibility that indicates whether the vertical scroll bar is visible. The
        //     default value is Visible.
        public Visibility ComputedVerticalScrollBarVisibility { get; }
        //
        // Summary:
        //     Identifies the ComputedVerticalScrollBarVisibility dependency property.
        //
        // Returns:
        //     The identifier for the ComputedVerticalScrollBarVisibility dependency property.
        public static DependencyProperty ComputedVerticalScrollBarVisibilityProperty { get; }
        //
        // Summary:
        //     Gets the vertical size of all the content for display in the ScrollViewer.
        //
        // Returns:
        //     The vertical size of all the content for display in the ScrollViewer.
        public double ExtentHeight { get; }
        //
        // Summary:
        //     Identifier for the ExtentHeight dependency property.
        //
        // Returns:
        //     The identifier for the ExtentHeight dependency property.
        public static DependencyProperty ExtentHeightProperty { get; }
        //
        // Summary:
        //     Gets the horizontal size of all the content for display in the ScrollViewer.
        //
        // Returns:
        //     The horizontal size of all the content for display in the ScrollViewer.
        public double ExtentWidth { get; }
        //
        // Summary:
        //     Identifier for the ExtentWidth dependency property.
        //
        // Returns:
        //     The identifier for the ExtentWidth dependency property.
        public static DependencyProperty ExtentWidthProperty { get; }
        //
        //
        // Summary:
        //     Identifies the HorizontalOffset dependency property.
        //
        // Returns:
        //     The identifier for the HorizontalOffset dependency property.
        public static DependencyProperty HorizontalOffsetProperty { get; }
         * */
        // Summary:
        //     Gets a value that indicates the horizontal offset of the scrolled content.
        //
        // Returns:
        //     The horizontal offset of the scrolled content.
        public double HorizontalOffset
        {
            get
            {
                // Note: we did not create a DependencyProperty because we do not want to slow down the scroll by calling SetValue during the scroll.
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                    _horizontalOffset = (double)INTERNAL_HtmlDomManager.GetDomElementAttribute(this.INTERNAL_OuterDomElement, "scrollLeft");
                return _horizontalOffset;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a horizontal ScrollBar should
        /// be displayed.
        /// 
        /// Returns a ScrollBarVisibility value that indicates whether a horizontal ScrollBar
        /// should be displayed. The default value is Hidden.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); } //todo: use this to set the appearance of the horizontalScrollBar for the content (probably using overflow from Html).
        }
        /// <summary>
        /// Identifies the HorizontalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer), new PropertyMetadata(ScrollBarVisibility.Disabled, HorizontalScrollBarVisibility_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        static void HorizontalScrollBarVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer)
            {
                var scrollViewer = (ScrollViewer)d;
                ScrollBarVisibility newValue = (ScrollBarVisibility)e.NewValue;

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(scrollViewer))
                {
                    //-----------------------------
                    // Gain access to the styles:
                    //-----------------------------

                    var outerDiv = scrollViewer.INTERNAL_OuterDomElement;
                    var outerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDiv);
                    var innerDiv = INTERNAL_HtmlDomManager.GetFirstChildDomElement(outerDiv);
                    var innerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(innerDiv);

                    //----------------
                    // Update the DOM:
                    //----------------

                    scrollViewer.INTERNAL_ApplyHorizontalSettings(newValue, outerDivStyle, innerDivStyle);
                }
            }
        }


        ///// <summary>
        ///// Gets or sets a value that determines how manipulation input influences scrolling
        ///// behavior on the horizontal axis.
        ///// 
        ///// Returns a value of the enumeration. The typical default (as set through the default
        ///// template, not class initialization) is Rails.
        ///// </summary>
        //public ScrollMode HorizontalScrollMode
        //{
        //    get { return (ScrollMode)GetValue(HorizontalScrollModeProperty); }
        //    set { SetValue(HorizontalScrollModeProperty, value); } //todo: use this to set the appearance of the horizontalScrollBar for the content (probably using overflow from Html).
        //}
        ///// <summary>
        ///// Identifies the HorizontalScrollMode dependency property.
        ///// </summary>
        //public static readonly DependencyProperty HorizontalScrollModeProperty =
        //    DependencyProperty.Register("HorizontalScrollMode", typeof(ScrollMode), typeof(ScrollViewer), new PropertyMetadata(ScrollMode.Auto, HorizontalScrollMode_Changed));
        //static void HorizontalScrollMode_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var border = (ScrollViewer)d;
        //    ScrollMode newValue = (ScrollMode)e.NewValue;
        //    if (INTERNAL_VisualTreeManager.IsElementInVisualTree(border))
        //    {
        //        //todo: fill this
        //    }
        //}




        /*
        //
        // Summary:
        //     Gets or sets a value that indicates how the existing snap points are horizontally
        //     aligned versus the initial viewport.
        //
        // Returns:
        //     A value of the enumeration.
        public SnapPointsAlignment HorizontalSnapPointsAlignment { get; set; }
        //
        // Summary:
        //     Identifies the HorizontalSnapPointsAlignment dependency property.
        //
        // Returns:
        //     The identifier for the HorizontalSnapPointsAlignment dependency property.
        public static DependencyProperty HorizontalSnapPointsAlignmentProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that declares how manipulation behavior reacts to the
        //     snap points along the horizontal axis.
        //
        // Returns:
        //     A value of the enumeration.
        public SnapPointsType HorizontalSnapPointsType { get; set; }
        //
        // Summary:
        //     Identifies the HorizontalSnapPointsType dependency property.
        //
        // Returns:
        //     The identifier for the HorizontalSnapPointsType dependency property.
        public static DependencyProperty HorizontalSnapPointsTypeProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that determines the deferred scrolling behavior for
        //     a ScrollViewer.
        //
        // Returns:
        //     True if deferred scrolling should occur; otherwise, false.
        public bool IsDeferredScrollingEnabled { get; set; }
        //
        // Summary:
        //     Identifies the IsDeferredScrollingEnabled dependency property.
        //
        // Returns:
        //     The identifier for the IsDeferredScrollingEnabled dependency property.
        public static DependencyProperty IsDeferredScrollingEnabledProperty { get; }
         * 


        /// <summary>
        /// Gets or sets a value that indicates whether the scroll rail is enabled for the horizontal axis.
        /// </summary>
        public bool IsHorizontalRailEnabled
        {
            get { return (bool)GetValue(IsHorizontalRailEnabledProperty); }
            set { SetValue(IsHorizontalRailEnabledProperty, value); } //todo: see the exact behaviour and see if it is possible to reproduce (may require to create our own Scrollbar or something since I read that the Scrollbar's style could not be changed)
        }
        public static readonly DependencyProperty IsHorizontalRailEnabledProperty =
            DependencyProperty.Register("IsHorizontalRailEnabled", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(true, IsHorizontalRailEnabled_Changed));


        static void IsHorizontalRailEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = (ScrollViewer)d;
            bool newValue = (bool)e.NewValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(border))
            {
                //todo: fill this
            }
        }

        
        //
        // Summary:
        //     Gets or sets a value that indicates whether scroll chaining is enabled from
        //     this child to its parent, for the horizontal axis.
        //
        // Returns:
        //     True to enable horizontal scroll chaining from child to parent; otherwise,
        //     false.
        public bool IsHorizontalScrollChainingEnabled { get; set; }
        //
        // Summary:
        //     Identifies the IsHorizontalScrollChainingEnabled dependency property.
        //
        // Returns:
        //     The identifier for the IsHorizontalScrollChainingEnabled dependency property.
        public static DependencyProperty IsHorizontalScrollChainingEnabledProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether scroll actions should include
        //     inertia in their behavior and value.
        //
        // Returns:
        //     True if scroll actions should include inertia in their behavior and value;
        //     otherwise, false.
        public bool IsScrollInertiaEnabled { get; set; }
        //
        // Summary:
        //     Identifies the IsScrollInertiaEnabled dependency property.
        //
        // Returns:
        //     The identifier for the IsScrollInertiaEnabled dependency property.
        public static DependencyProperty IsScrollInertiaEnabledProperty { get; }
        
        /// <summary>
        /// Gets or sets a value that indicates whether the scroll rail is enabled for the vertical axis.
        /// </summary>
        public bool IsVerticalRailEnabled
        {
            get { return (bool)GetValue(IsVerticalRailEnabledProperty); }
            set { SetValue(IsVerticalRailEnabledProperty, value); } //todo: see the exact behaviour and see if it is possible to reproduce (may require to create our own Scrollbar or something since I read that the Scrollbar's style could not be changed)
        }
        public static readonly DependencyProperty IsVerticalRailEnabledProperty =
            DependencyProperty.Register("IsVerticalRailEnabled", typeof(bool), typeof(ScrollViewer), new PropertyMetadata(true, IsVerticalRailEnabled_Changed));


        static void IsVerticalRailEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = (ScrollViewer)d;
            bool newValue = (bool)e.NewValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(border))
            {
                //todo: fill this
            }
        }


        
        //
        // Summary:
        //     Gets or sets a value that indicates whether scroll chaining is enabled from
        //     this child to its parent, for the vertical axis.
        //
        // Returns:
        //     True to enable vertical scroll chaining from child to parent; otherwise,
        //     false.
        public bool IsVerticalScrollChainingEnabled { get; set; }
        //
        // Summary:
        //     Identifies the IsVerticalScrollChainingEnabled dependency property.
        //
        // Returns:
        //     The identifier for the IsVerticalScrollChainingEnabled dependency property.
        public static DependencyProperty IsVerticalScrollChainingEnabledProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether zoom chaining is enabled from
        //     this child to its parent.
        //
        // Returns:
        //     True to enable zoom chaining from child to parent; otherwise, false.
        public bool IsZoomChainingEnabled { get; set; }
        //
        // Summary:
        //     Identifies the IsZoomChainingEnabled dependency property.
        //
        // Returns:
        //     The identifier for the IsZoomChainingEnabled dependency property.
        public static DependencyProperty IsZoomChainingEnabledProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether zoom actions should include inertia
        //     in their behavior and value.
        //
        // Returns:
        //     True if zoom actions should include inertia in their behavior and value;
        //     otherwise, false.
        public bool IsZoomInertiaEnabled { get; set; }
        //
        // Summary:
        //     Identifies the IsZoomInertiaEnabled dependency property.
        //
        // Returns:
        //     The identifier for the IsZoomInertiaEnabled dependency property.
        public static DependencyProperty IsZoomInertiaEnabledProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that indicates the maximum permitted run-time value
        //     of ZoomFactor.
        //
        // Returns:
        //     The maximum permitted run-time value of ZoomFactor. The default is 10.
        public float MaxZoomFactor { get; set; }
        //
        // Summary:
        //     Identifies the MaxZoomFactor dependency property.
        //
        // Returns:
        //     The identifier for the MaxZoomFactor dependency property.
        public static DependencyProperty MaxZoomFactorProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that indicates the minimum permitted run-time value
        //     of ZoomFactor.
        //
        // Returns:
        //     The minimum permitted run-time value of ZoomFactor. The default is 0.1.
        public float MinZoomFactor { get; set; }
        //
        // Summary:
        //     Identifies the MinZoomFactor dependency property.
        //
        // Returns:
        //     The identifier for the MinZoomFactor dependency property.
        public static DependencyProperty MinZoomFactorProperty { get; }
        //
        // Summary:
        //     Gets a value that represents the vertical size of the area that can be scrolled;
        //     the difference between the width of the extent and the width of the viewport.
        //
        // Returns:
        //     The vertical size of the area that can be scrolled. This property has no
        //     default value.
        public double ScrollableHeight { get; }
        //
        // Summary:
        //     Identifies the ScrollableHeight dependency property.
        //
        // Returns:
        //     The identifier for the ScrollableHeight dependency property.
        public static DependencyProperty ScrollableHeightProperty { get; }
        //
        // Summary:
        //     Gets a value that represents the horizontal size of the area that can be
        //     scrolled; the difference between the width of the extent and the width of
        //     the viewport.
        //
        // Returns:
        //     The horizontal size of the area that can be scrolled. This property has no
        //     default value.
        public double ScrollableWidth { get; }
        //
        // Summary:
        //     Identifies the ScrollableWidth dependency property.
        //
        // Returns:
        //     The identifier for the ScrollableWidth dependency property.
        public static DependencyProperty ScrollableWidthProperty { get; }
        //
        // Summary:
        //     Gets a value that indicates the vertical offset of the scrolled content.
        //
        // Returns:
        //     The vertical offset of the scrolled content.
        public double VerticalOffset { get; }
        //
        // Summary:
        //     Identifies the VerticalOffset dependency property.
        //
        // Returns:
        //     The identifier for the VerticalOffset dependency property.
        public static DependencyProperty VerticalOffsetProperty { get; }
        */
        /// <summary>
        ///  Gets a value that indicates the vertical offset of the scrolled content.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                // Note: we did not create a DependencyProperty because we do not want to slow down the scroll by calling SetValue during the scroll.
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                    _verticalOffset = (double)INTERNAL_HtmlDomManager.GetDomElementAttribute(this.INTERNAL_OuterDomElement, "scrollTop");
                return _verticalOffset;
            }
        }


        /// <summary>
        /// Gets or sets a value that indicates whether a vertical ScrollBar should be displayed.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); } //todo: use this to set the appearance of the horizontalScrollBar for the content (probably using overflow from Html).
        }
        /// <summary>
        /// Identifies the VerticalScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer), new PropertyMetadata(ScrollBarVisibility.Visible, VerticalScrollBarVisibility_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        
        static void VerticalScrollBarVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer)
            {
                var scrollViewer = (ScrollViewer)d;
                ScrollBarVisibility newValue = (ScrollBarVisibility)e.NewValue;

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(scrollViewer))
                {
                    //-----------------------------
                    // Gain access to the styles:
                    //-----------------------------

                    var outerDiv = scrollViewer.INTERNAL_OuterDomElement;
                    var outerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDiv);
                    var innerDiv = INTERNAL_HtmlDomManager.GetFirstChildDomElement(outerDiv);
                    var innerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(innerDiv);

                    //----------------
                    // Update the DOM:
                    //----------------

                    scrollViewer.INTERNAL_ApplyVerticalSettings(newValue, outerDivStyle, innerDivStyle);
                }
            }
        }

        ///// <summary>
        ///// Gets or sets a value that determines how manipulation input influences scrolling behavior on the vertical axis.
        ///// </summary>
        //public ScrollMode VerticalScrollMode
        //{
        //    get { return (ScrollMode)GetValue(VerticalScrollModeProperty); }
        //    set { SetValue(VerticalScrollModeProperty, value); } //todo: use this to set the appearance of the horizontalScrollBar for the content (probably using overflow from Html).
        //}
        ///// <summary>
        ///// Identifies the VerticalScrollMode dependency property.
        ///// </summary>
        //public static readonly DependencyProperty VerticalScrollModeProperty =
        //    DependencyProperty.Register("VerticalScrollMode", typeof(ScrollMode), typeof(ScrollViewer), new PropertyMetadata(ScrollMode.Auto, VerticalScrollMode_Changed)); //todo: see if this should have null as a default value since the description says that the default value is set in the Default template


        //static void VerticalScrollMode_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var border = (ScrollViewer)d;
        //    ScrollMode newValue = (ScrollMode)e.NewValue;
        //    if (INTERNAL_VisualTreeManager.IsElementInVisualTree(border))
        //    {
        //        //todo: fill this
        //    }
        //}

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            object outerDiv;
            dynamic outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
            outerDivStyle.height = "100%";
            outerDivStyle.width = "100%";
            outerDivStyle.overflowX = "scroll";
            outerDivStyle.overflowY = "scroll";
            //Update the scrollviewer position when we insert again the scrollviewer in the visual tree
            if (_verticalOffset != 0)
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(outerDiv, "scrollTop", _verticalOffset);
            }
            if (_horizontalOffset != 0)
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(outerDiv, "scrollLeft", _horizontalOffset);
            }


            object innerDiv;
            dynamic innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out innerDiv);
            innerDivStyle.position = "relative";

            // Note: the "height" and "width" of the innerDiv are handled in the methods "INTERNAL_ApplyHorizontalSettings" and "INTERNAL_ApplyVerticalSettings".

            INTERNAL_ApplyHorizontalSettings(this.HorizontalScrollBarVisibility, outerDivStyle, innerDivStyle);
            INTERNAL_ApplyVerticalSettings(this.VerticalScrollBarVisibility, outerDivStyle, innerDivStyle);

            domElementWhereToPlaceChildren = innerDiv;
            return outerDiv;
        }

        private void INTERNAL_ApplyHorizontalSettings(
            ScrollBarVisibility horizontalScrollBarVisibility,
            object outerDivStyle,
            object innerDivStyle)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                //-------------------------------------
                // Handle the "Overflow X" CSS property:
                //-------------------------------------

                switch (horizontalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Disabled:
                        //there cannot be an overflow at all
                        //a solution would be to put for ALL the children : max-width:100% (But it wouldn't work with margins)
                        //the solution above should stop when a child is a canvas : its children can have the size they want

                        ((dynamic)outerDivStyle).overflowX = "hidden";//todo: fix this (the children are not limited to the size of this control)
                        break;
                    case ScrollBarVisibility.Auto:
                        //there is a scrollbar when the content is wider, there is no scrollbar when the content fits
                        ((dynamic)outerDivStyle).overflowX = "auto"; //todo: check if the overflowX actually sets overflow-x and not overflow (I think it sets overflow).

                        break;
                    case ScrollBarVisibility.Hidden:
                        //--> overflow is hidden
                        ((dynamic)outerDivStyle).overflowX = "hidden";
                        break;
                    case ScrollBarVisibility.Visible:
                        ((dynamic)outerDivStyle).overflowX = "scroll";

                        try // Prevents crash in the simulator that uses IE.
                        {
                            ((dynamic)outerDivStyle).WebkitOverflowScrolling = "touch"; // Provides inertia smooth scrolling in Safari
                        }
                        catch
                        {
                            //do nothing
                        }
                        break;
                    default:
                        break;
                }

                //-----------------------------------------------------------------
                // Handle the "width" and "height" CSS properties of the Inner DIV:
                //-----------------------------------------------------------------

                if ((horizontalScrollBarVisibility == ScrollBarVisibility.Visible || horizontalScrollBarVisibility == ScrollBarVisibility.Auto) && double.IsNaN(this.Width)) //todo: update the ScrollViewer of its "width" changes.
                {
                    if (this.INTERNAL_VisualParent is Grid && !Grid_InternalHelpers.isCSSGridSupported()) //note: currently, only the non-CSS Grid uses the Dom element <table>
                    {
                        ((dynamic)innerDivStyle).width = "0px"; //we set it that way so that we still have a scrollbar even when in a table (table is unable to limit the size of its content)
                        ((dynamic)innerDivStyle).overflowX = "visible"; //so that its content can still be seen
                    }
                    else
                    {
                        ((dynamic)innerDivStyle).width = "auto"; // This makes it possible to center-align an item inside the ScrollViewer.
                        ((dynamic)innerDivStyle).minWidth = "100%"; // This fixes an issue where the background of the "InputProject" test project is correct only at the top of the page.
                    }
                }
                else
                {
                    ((dynamic)innerDivStyle).width = "100%";
                }
            }
        }

        private void INTERNAL_ApplyVerticalSettings(
            ScrollBarVisibility verticalScrollBarVisibility,
            dynamic outerDivStyle,
            dynamic innerDivStyle)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                //-------------------------------------
                // Handle the "Overflow Y" CSS property:
                //-------------------------------------

                switch (verticalScrollBarVisibility)
                {
                    case ScrollBarVisibility.Disabled:
                        //there cannot be an overflow at all
                        //a solution would be to put for ALL the children : max-width:100% (But it wouldn't work with margins)
                        //the solution above should stop when a child is a canvas : its children can have the size they want

                        outerDivStyle.overflowY = "hidden";//todo: fix this (the children are not limited to the size of this control)
                        break;
                    case ScrollBarVisibility.Auto:
                        //there is a scrollbar when the content is wider, there is no scrollbar when the content fits
                        outerDivStyle.overflowY = "auto"; //todo: check if the overflowX actually sets overflow-x and not overflow (I think it sets overflow).

                        break;
                    case ScrollBarVisibility.Hidden:
                        //--> overflow is hidden
                        outerDivStyle.overflowY = "hidden";
                        break;
                    case ScrollBarVisibility.Visible:
                        outerDivStyle.overflowY = "scroll";

                        try // Prevents crash in the simulator that uses IE.
                        {
                            outerDivStyle.WebkitOverflowScrolling = "touch"; // Provides inertia smooth scrolling in Safari
                        }
                        catch
                        {
                            //do nothing
                        }
                        break;
                    default:
                        break;
                }

                //-----------------------------------------------------------------
                // Handle the "width" and "height" CSS properties of the Inner DIV:
                //-----------------------------------------------------------------

                if ((verticalScrollBarVisibility == ScrollBarVisibility.Visible || verticalScrollBarVisibility == ScrollBarVisibility.Auto)
                    && double.IsNaN(this.Height) //todo: update the ScrollViewer of its "height" changes.
                    )
                {
                    if (this.INTERNAL_VisualParent is Grid && !Grid_InternalHelpers.isCSSGridSupported()) //note: currently, only the non-CSS Grid uses the Dom element <table>
                    {
                        innerDivStyle.height = "0px"; //we set it that way so that we still have a scrollbar even when in a table (table is unable to limit the size of its content)
                        innerDivStyle.overflowY = "visible"; //so that its content can still be seen
                    }
                    else
                    {
                        innerDivStyle.height = "auto"; // This makes it possible to center-align an item inside the ScrollViewer.
                        innerDivStyle.minHeight = "100%"; // This fixes an issue where the background of the "InputProject" test project is correct only at the top of the page.
                    }
                }
                else
                {
                    innerDivStyle.height = "100%";
                }
            }
        }

        /// <summary>
        /// Scrolls the content that is within the ScrollViewer to the specified horizontal offset position.
        /// </summary>
        /// <param name="offset">The position that the content scrolls to.</param>
        public void ScrollToHorizontalOffset(double offset)
        {
            //_horizontalOffset is there so we can remember changes of the value even if the ScrollViewer is not in the visual tree
            _horizontalOffset = offset;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                INTERNAL_HtmlDomManager.SetDomElementProperty(this.INTERNAL_OuterDomElement, "scrollLeft", offset);
        }

        /// <summary>
        /// Scrolls the content that is within the ScrollViewer to the specified vertical offset position.
        /// </summary>
        /// <param name="offset">The position that the content scrolls to.</param>
        public void ScrollToVerticalOffset(double offset)
        {
            //_verticalOffset is there so we can remember changes of the value even if the ScrollViewer is not in the visual tree
            _verticalOffset = offset;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                INTERNAL_HtmlDomManager.SetDomElementProperty(this.INTERNAL_OuterDomElement, "scrollTop", offset);
        }

#if WORKINPROGRESS
        // Summary:
        //     Gets the value of the HorizontalScrollBarVisibility dependency property /
        //     ScrollViewer.HorizontalScrollBarVisibility XAML attached property from a
        //     specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(DependencyObject element)
        {
            return (ScrollBarVisibility)element.GetValue(HorizontalScrollBarVisibilityProperty);
        }

        // Summary:
        //     Sets the value of the HorizontalScrollBarVisibility dependency property /
        //     ScrollViewer.HorizontalScrollBarVisibility XAML attached property on a specified
        //     element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   horizontalScrollBarVisibility:
        //     The value to set.
        public static void SetHorizontalScrollBarVisibility(DependencyObject element, ScrollBarVisibility horizontalScrollBarVisibility)
        {
            element.SetValue(HorizontalScrollBarVisibilityProperty, horizontalScrollBarVisibility);
        }

        // Summary:
        //     Gets the value of the VerticalScrollBarVisibility dependency property / ScrollViewer.VerticalScrollBarVisibility
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static ScrollBarVisibility GetVerticalScrollBarVisibility(DependencyObject element)
        {
            return (ScrollBarVisibility)element.GetValue(VerticalScrollBarVisibilityProperty);
        }

        // Summary:
        //     Sets the value of the VerticalScrollBarVisibility dependency property / ScrollViewer.VerticalScrollBarVisibility
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   verticalScrollBarVisibility:
        //     The value to set.
        public static void SetVerticalScrollBarVisibility(DependencyObject element, ScrollBarVisibility verticalScrollBarVisibility)
        {
            element.SetValue(VerticalScrollBarVisibilityProperty, verticalScrollBarVisibility);
        }
#endif
        /*
        //
        // Summary:
        //     Gets or sets a value that indicates how the existing snap points are vertically
        //     aligned versus the initial viewport.
        //
        // Returns:
        //     A value of the enumeration.
        public SnapPointsAlignment VerticalSnapPointsAlignment { get; set; }
        //
        // Summary:
        //     Identifies the VerticalSnapPointsAlignment dependency property.
        //
        // Returns:
        //     The identifier for the VerticalSnapPointsAlignment dependency property.
        public static DependencyProperty VerticalSnapPointsAlignmentProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that declares how manipulation behavior reacts to the
        //     snap points along the vertical axis.
        //
        // Returns:
        //     A value of the enumeration.
        public SnapPointsType VerticalSnapPointsType { get; set; }
        //
        // Summary:
        //     Identifies the VerticalSnapPointsType dependency property.
        //
        // Returns:
        //     The identifier for the VerticalSnapPointsType dependency property.
        public static DependencyProperty VerticalSnapPointsTypeProperty { get; }
        //
        // Summary:
        //     Gets a value that contains the vertical size of the viewable content.
        //
        // Returns:
        //     The vertical size of the viewable content. This property has no default value.
        public double ViewportHeight { get; }
        //
        // Summary:
        //     Identifies the ViewportHeight dependency property.
        //
        // Returns:
        //     The identifier for the ViewportHeight dependency property.
        public static DependencyProperty ViewportHeightProperty { get; }
        //
        // Summary:
        //     Gets a value that contains the horizontal size of the viewable content.
        //
        // Returns:
        //     The horizontal size of the viewable content. The default value is 0.0.
        public double ViewportWidth { get; }
        //
        // Summary:
        //     Identifies the ViewportWidth dependency property.
        //
        // Returns:
        //     The identifier for the ViewportWidth dependency property.
        public static DependencyProperty ViewportWidthProperty { get; }
        //
        // Summary:
        //     Gets a value that indicates the current zoom factor engaged for content scaling.
        //
        // Returns:
        //     The current zoom factor engaged for content scaling. The default is 1.0,
        //     where 1.0 indicates no additional scaling.
        public float ZoomFactor { get; }
        //
        // Summary:
        //     Identifies the ZoomFactor dependency property.
        //
        // Returns:
        //     The identifier for the ZoomFactor dependency property.
        public static DependencyProperty ZoomFactorProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that indicates whether zoom behavior in the ScrollViewer
        //     content is enabled.
        //
        // Returns:
        //     A value of the enumeration.
        public ZoomMode ZoomMode { get; set; }
        //
        // Summary:
        //     Identifies the ZoomMode dependency property.
        //
        // Returns:
        //     The identifier for the ZoomMode dependency property.
        public static DependencyProperty ZoomModeProperty { get; }
        //
        // Summary:
        //     Gets the observable collection of zoom snap point factors that are held by
        //     the ScrollViewer.
        //
        // Returns:
        //     A collection of Single values that represent zoom factors as snap points.
        public IList<float> ZoomSnapPoints { get; }
        //
        // Summary:
        //     Identifies the ZoomSnapPoints dependency property.
        //
        // Returns:
        //     The identifier for the ZoomSnapPoints dependency property.
        public static DependencyProperty ZoomSnapPointsProperty { get; }
        //
        // Summary:
        //     Gets or sets a value that indicates how zoom snap points are processed for
        //     interaction input.
        //
        // Returns:
        //     A value of the enumeration.
        public SnapPointsType ZoomSnapPointsType { get; set; }
        //
        // Summary:
        //     Identifies the ZoomSnapPointsType dependency property.
        //
        // Returns:
        //     The identifier for the ZoomSnapPointsType dependency property.
        public static DependencyProperty ZoomSnapPointsTypeProperty { get; }

        // Summary:
        //     Occurs when manipulations that affect the view raise an underlying event.
        public event EventHandler<ScrollViewerViewChangedEventArgs> ViewChanged;

        public static bool GetBringIntoViewOnFocusChange(DependencyObject element);
        //
        //
        // Summary:
        //     Gets the value of the HorizontalScrollMode dependency property / ScrollViewer.HorizontalScrollMode
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static ScrollMode GetHorizontalScrollMode(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsDeferredScrollingEnabled dependency property / ScrollViewer.IsDeferredScrollingInertiaEnabled
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsDeferredScrollingEnabled(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsHorizontalRailEnabled dependency property / ScrollViewer.IsHorizontalRailEnabled
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsHorizontalRailEnabled(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsHorizontalScrollChainingEnabled dependency property
        //     / ScrollViewer.IsHorizontalScrollChainingEnabled XAML attached property from
        //     a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsHorizontalScrollChainingEnabled(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsScrollInertiaEnabled dependency property / ScrollViewer.IsScrollInertiaEnabled
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsScrollInertiaEnabled(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsVerticalRailEnabled dependency property / ScrollViewer.IsVerticalRailEnabled
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsVerticalRailEnabled(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsVerticalScrollChainingEnabled dependency property
        //     / ScrollViewer.IsVerticalScrollChainingEnabled XAML attached property from
        //     a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsVerticalScrollChainingEnabled(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsZoomChainingEnabled dependency property / ScrollViewer.IsZoomChainingEnabled
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsZoomChainingEnabled(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the IsZoomInertiaEnabled dependency property / ScrollViewer.IsZoomInertiaEnabled
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static bool GetIsZoomInertiaEnabled(DependencyObject element);
        //
        //
        // Summary:
        //     Gets the value of the VerticalScrollMode dependency property / ScrollViewer.VerticalScrollMode
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static ScrollMode GetVerticalScrollMode(DependencyObject element);
        //
        // Summary:
        //     Gets the value of the ZoomMode dependency property / ScrollViewer.ZoomMode
        //     XAML attached property from a specified element.
        //
        // Parameters:
        //   element:
        //     The element from which the property value is read.
        //
        // Returns:
        //     The value of the property, as obtained from the property store.
        public static ZoomMode GetZoomMode(DependencyObject element);
        //
        // Summary:
        //     Called when the value of properties that describe the size and location of
        //     the scroll area change.
        public void InvalidateScrollInfo();
        public static void SetBringIntoViewOnFocusChange(DependencyObject element, bool bringIntoViewOnFocusChange);
        //
        //
        // Summary:
        //     Sets the value of the HorizontalScrollMode dependency property / ScrollViewer.HorizontalScrollMode
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   horizontalScrollMode:
        //     The value to set.
        public static void SetHorizontalScrollMode(DependencyObject element, ScrollMode horizontalScrollMode);
        //
        // Summary:
        //     Sets the value of the IsDeferredScrollingEnabled dependency property / ScrollViewer.IsDeferredScrollingEnabled
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isDeferredScrollingEnabled:
        //     The value to set.
        public static void SetIsDeferredScrollingEnabled(DependencyObject element, bool isDeferredScrollingEnabled);
        //
        // Summary:
        //     Sets the value of the IsHorizontalRailEnabled dependency property / ScrollViewer.IsHorizontalRailEnabled
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isHorizontalRailEnabled:
        //     The value to set.
        public static void SetIsHorizontalRailEnabled(DependencyObject element, bool isHorizontalRailEnabled);
        //
        // Summary:
        //     Sets the value of the IsHorizontalScrollChainingEnabled dependency property
        //     / ScrollViewer.IsHorizontalScrollChainingEnabled XAML attached property on
        //     a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isHorizontalScrollChainingEnabled:
        //     The value to set.
        public static void SetIsHorizontalScrollChainingEnabled(DependencyObject element, bool isHorizontalScrollChainingEnabled);
        //
        // Summary:
        //     Sets the value of the IsScrollInertiaEnabled dependency property / ScrollViewer.IsScrollInertiaEnabled
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isScrollInertiaEnabled:
        //     The value to set.
        public static void SetIsScrollInertiaEnabled(DependencyObject element, bool isScrollInertiaEnabled);
        //
        // Summary:
        //     Sets the value of the IsVerticalRailEnabled dependency property / ScrollViewer.IsVerticalRailEnabled
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isVerticalRailEnabled:
        //     The value to set.
        public static void SetIsVerticalRailEnabled(DependencyObject element, bool isVerticalRailEnabled);
        //
        // Summary:
        //     Sets the value of the IsVerticalScrollChainingEnabled dependency property
        //     / ScrollViewer.IsVerticalScrollChainingEnabled XAML attached property on
        //     a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isVerticalScrollChainingEnabled:
        //     The value to set.
        public static void SetIsVerticalScrollChainingEnabled(DependencyObject element, bool isVerticalScrollChainingEnabled);
        //
        // Summary:
        //     Sets the value of the IsZoomChainingEnabled dependency property / ScrollViewer.IsZoomChainingEnabled
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isZoomChainingEnabled:
        //     The value to set.
        public static void SetIsZoomChainingEnabled(DependencyObject element, bool isZoomChainingEnabled);
        //
        // Summary:
        //     Sets the value of the IsZoomInertiaEnabled dependency property / ScrollViewer.IsZoomInertiaEnabled
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   isZoomInertiaEnabled:
        //     The value to set.
        public static void SetIsZoomInertiaEnabled(DependencyObject element, bool isZoomInertiaEnabled);
        //
        //
        // Summary:
        //     Sets the value of the VerticalScrollMode dependency property / ScrollViewer.VerticalScrollMode
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   verticalScrollMode:
        //     The value to set.
        public static void SetVerticalScrollMode(DependencyObject element, ScrollMode verticalScrollMode);
        //
        // Summary:
        //     Sets the value of the ZoomMode dependency property / ScrollViewer.ZoomMode
        //     XAML attached property on a specified element.
        //
        // Parameters:
        //   element:
        //     The element on which to set the property value.
        //
        //   zoomMode:
        //     The value to set.
        public static void SetZoomMode(DependencyObject element, ZoomMode zoomMode);
        //
        // Summary:
        //     Sets the effective value of ZoomFactor.
        //
        // Parameters:
        //   factor:
        //     The zoom factor to set. The factors are based on a norm of 1.0, which represents
        //     no zoom applied. The values you can set are also influenced by current values
        //     for MinZoomFactor and MaxZoomFactor.
        public void ZoomToFactor(float factor);

        */

#if WORKINPROGRESS
        public static readonly DependencyProperty ScrollableHeightProperty = DependencyProperty.Register("ScrollableHeight", typeof(double), typeof(ScrollViewer), null);

        public double ScrollableHeight
        {
            get { return (double)this.GetValue(ScrollableHeightProperty); }
            private set { this.SetValue(ScrollableHeightProperty, value); }
        }

        public static readonly DependencyProperty ScrollableWidthProperty = DependencyProperty.Register("ScrollableWidth", typeof(double), typeof(ScrollViewer), null);

        public double ScrollableWidth
        {
            get { return (double)this.GetValue(ScrollableWidthProperty); }
            private set { this.SetValue(ScrollableWidthProperty, value); }
        }

        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof(double), typeof(ScrollViewer), null);

        public double ViewportHeight
        {
            get { return (double)this.GetValue(ViewportHeightProperty); }
            private set { this.SetValue(ViewportHeightProperty, value); }
        }

        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register("ViewportWidth", typeof(double), typeof(ScrollViewer), null);

        public double ViewportWidth
        {
            get { return (double)this.GetValue(ViewportWidthProperty); }
            private set { this.SetValue(ViewportWidthProperty, value); }
        }

        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), null);

        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return (Visibility)this.GetValue(ComputedHorizontalScrollBarVisibilityProperty); }
            private set { this.SetValue(ComputedHorizontalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty = DependencyProperty.Register("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), null);

        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return (Visibility)this.GetValue(ComputedVerticalScrollBarVisibilityProperty); }
            private set { this.SetValue(ComputedVerticalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ExtentHeightProperty = DependencyProperty.Register("ExtentHeight", typeof(double), typeof(ScrollViewer), null);

        public double ExtentHeight
        {
            get { return (double)this.GetValue(ExtentHeightProperty); }
            private set { this.SetValue(ExtentHeightProperty, value); }
        }

        public static readonly DependencyProperty ExtentWidthProperty = DependencyProperty.Register("ExtentWidth", typeof(double), typeof(ScrollViewer), null);

        public double ExtentWidth
        {
            get { return (double)this.GetValue(ExtentWidthProperty); }
            private set { this.SetValue(ExtentWidthProperty, value); }
        }

        public void InvalidateScrollInfo()
        {

        }
#endif
    }
}
