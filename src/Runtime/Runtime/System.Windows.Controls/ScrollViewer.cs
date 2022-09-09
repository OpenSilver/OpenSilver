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
using System;
using System.Collections.Generic;
using System.Linq;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

#endif

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
        private const string ElementScrollContentPresenterName = "ScrollContentPresenter";
        private const string ElementHorizontalScrollBarName = "HorizontalScrollBar";
        private const string ElementVerticalScrollBarName = "VerticalScrollBar";

        double _verticalOffset = 0;
        double _horizontalOffset = 0;
        /// <summary>
        /// This variable is used to know whether the horizontal offset (through scrollLeft on the DOM element) is properly applied.
        /// It is needed because scrollLeft is ignored when the display is set to none, so if the Visibility is Collapsed, we need to defer applying it until after the Visibility changes to Visible.
        /// </summary>
        bool _isHorizontalOffsetInvalid = false;
        /// <summary>
        /// This variable is used to know whether the vertical offset (through scrollTop on the DOM element) is properly applied.
        /// It is needed because scrollTop is ignored when the display is set to none, so if the Visibility is Collapsed, we need to defer applying it until after the Visibility changes to Visible.
        /// </summary>
        bool _isVerticalOffsetInvalid = false;


        /// <summary>
        /// Initializes a new instance of the ScrollViewer class.
        /// </summary>
        public ScrollViewer()
        {
            DefaultStyleKey = typeof(ScrollViewer);

            IsVisibleChanged += ScrollViewer_IsVisibleChanged;
        }

        private void ScrollViewer_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                UpdateDomHorizontalOffset(this.INTERNAL_OuterDomElement);
                UpdateDomVerticalOffset(this.INTERNAL_OuterDomElement);
            }
        }

        internal override FrameworkTemplate TemplateCache
        {
            get
            {
                if (IsCustomLayoutRoot || IsUnderCustomLayout)
                {
                    return base.TemplateCache;
                }

                return null;
            }
            set
            {
                base.TemplateCache = value;
            }
        }

        internal override FrameworkTemplate TemplateInternal
        {
            get
            {
                if (IsCustomLayoutRoot || IsUnderCustomLayout)
                {
                    return base.TemplateInternal;
                }

                return null;
            }
        }

        // Note: we need to force this to true because template are disabled
        // for ScrollViewers.
        internal override bool EnablePointerEventsCore
        {
            get
            {
                return true;
            }
        }

        /// <summary> 
        /// Reference to the ScrollContentPresenter child.
        /// </summary>
        internal ScrollContentPresenter ElementScrollContentPresenter { get; private set; }

        /// <summary> 
        /// Reference to the horizontal ScrollBar child. 
        /// </summary>
        internal ScrollBar ElementHorizontalScrollBar { get; private set; }

        /// <summary> 
        /// Reference to the vertical ScrollBar child.
        /// </summary>
        internal ScrollBar ElementVerticalScrollBar { get; private set; }

        internal IScrollInfo ScrollInfo
        {
            get; set;
        }

        /// <summary>
        /// Gets the value of the horizontal offset of the content. 
        /// </summary> 
        public double HorizontalOffset
        {
            get
            {
                if (this.IsCustomLayoutRoot || this.IsUnderCustomLayout)
                {
                    return (double)GetValue(HorizontalOffsetProperty);
                }

                // Note: we did not create a DependencyProperty because we do not want to slow down the scroll by calling SetValue during the scroll.
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    try
                    {
                        _horizontalOffset = Convert.ToDouble(OpenSilver.Interop.ExecuteJavaScript("$0[$1]", this.INTERNAL_OuterDomElement, "scrollLeft"));
                    }
                    catch (InvalidCastException)
                    {
                    }
                }
                return _horizontalOffset;
            }
            private set { SetValue(HorizontalOffsetProperty, value); }
        }

        /// <summary> 
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
            "HorizontalOffset", typeof(double), typeof(ScrollViewer),
            new PropertyMetadata(new PropertyChangedCallback(OnScrollInfoDependencyPropertyChanged)));

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
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer),
                new FrameworkPropertyMetadata(ScrollBarVisibility.Disabled, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, HorizontalScrollBarVisibility_Changed)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        static void HorizontalScrollBarVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer)
            {
                var scrollViewer = (ScrollViewer)d;
                ScrollBarVisibility newValue = (ScrollBarVisibility)e.NewValue;

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(scrollViewer) && !scrollViewer.IsUnderCustomLayout)
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

        private static void OnScrollInfoDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

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
        */


        /// <summary> 
        /// Gets the value of the vertical offset of the content.
        /// </summary> 
        public double VerticalOffset
        {
            get
            {
                if (this.IsCustomLayoutRoot || this.IsUnderCustomLayout)
                {
                    return (double)GetValue(VerticalOffsetProperty);
                }

                // Note: we did not create a DependencyProperty because we do not want to slow down the scroll by calling SetValue during the scroll.
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    try
                    {
                        _verticalOffset = Convert.ToDouble(OpenSilver.Interop.ExecuteJavaScript("$0[$1]", this.INTERNAL_OuterDomElement, "scrollTop"));
                    }
                    catch (InvalidCastException)
                    {
                    }
                }
                return _verticalOffset;
            }
            private set { SetValue(VerticalOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalOffset dependency property. 
        /// </summary> 
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
            "VerticalOffset", typeof(double), typeof(ScrollViewer),
            new PropertyMetadata(new PropertyChangedCallback(OnScrollInfoDependencyPropertyChanged)));


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
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer),
                new FrameworkPropertyMetadata(ScrollBarVisibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure, VerticalScrollBarVisibility_Changed)
                { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void VerticalScrollBarVisibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer)
            {
                var scrollViewer = (ScrollViewer)d;
                ScrollBarVisibility newValue = (ScrollBarVisibility)e.NewValue;

                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(scrollViewer) && !scrollViewer.IsUnderCustomLayout)
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
            var outerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", parentRef, this, out outerDiv);
            outerDivStyle.height = "100%";
            outerDivStyle.width = "100%";

            if (!IsCustomLayoutRoot && !IsUnderCustomLayout)
            {
                outerDivStyle.overflowX = "scroll";
                outerDivStyle.overflowY = "scroll";
            }

            //Update the scrollviewer position when we insert again the scrollviewer in the visual tree
            if (_verticalOffset != 0)
            {
                UpdateDomVerticalOffset(outerDiv);
            }
            if (_horizontalOffset != 0)
            {
                UpdateDomHorizontalOffset(outerDiv);
            }


            object innerDiv;
            var innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out innerDiv);
            innerDivStyle.position = "relative";

            // Note: the "height" and "width" of the innerDiv are handled in the methods "INTERNAL_ApplyHorizontalSettings" and "INTERNAL_ApplyVerticalSettings".

            INTERNAL_ApplyHorizontalSettings(this.HorizontalScrollBarVisibility, outerDivStyle, innerDivStyle);
            INTERNAL_ApplyVerticalSettings(this.VerticalScrollBarVisibility, outerDivStyle, innerDivStyle);

            domElementWhereToPlaceChildren = innerDiv;
            return outerDiv;
        }

#if OPENSILVER
        private void INTERNAL_ApplyHorizontalSettings(
            ScrollBarVisibility horizontalScrollBarVisibility,
            INTERNAL_HtmlDomStyleReference outerDivStyle,
            INTERNAL_HtmlDomStyleReference innerDivStyle)
#elif BRIDGE
        private void INTERNAL_ApplyHorizontalSettings(
            ScrollBarVisibility horizontalScrollBarVisibility,
            dynamic outerDivStyle,
            dynamic innerDivStyle)
#endif
        {
            // if it's under customlayout, it works with Measure & Arrange.
            if (this.IsCustomLayoutRoot || this.IsUnderCustomLayout)
                return;

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

                        outerDivStyle.overflowX = "hidden";//todo: fix this (the children are not limited to the size of this control)
                        break;
                    case ScrollBarVisibility.Auto:
                        //there is a scrollbar when the content is wider, there is no scrollbar when the content fits
                        outerDivStyle.overflowX = "auto"; //todo: check if the overflowX actually sets overflow-x and not overflow (I think it sets overflow).

                        break;
                    case ScrollBarVisibility.Hidden:
                        //--> overflow is hidden
                        outerDivStyle.overflowX = "hidden";
                        break;
                    case ScrollBarVisibility.Visible:
                        outerDivStyle.overflowX = "scroll";

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

                if ((horizontalScrollBarVisibility == ScrollBarVisibility.Visible || horizontalScrollBarVisibility == ScrollBarVisibility.Auto) && double.IsNaN(this.Width)) //todo: update the ScrollViewer of its "width" changes.
                {
                    if (this.INTERNAL_VisualParent is Grid && !Grid_InternalHelpers.isCSSGridSupported()) //note: currently, only the non-CSS Grid uses the Dom element <table>
                    {
                        innerDivStyle.width = "0px"; //we set it that way so that we still have a scrollbar even when in a table (table is unable to limit the size of its content)
                        innerDivStyle.overflowX = "visible"; //so that its content can still be seen
                    }
                    else
                    {
                        innerDivStyle.width = "auto"; // This makes it possible to center-align an item inside the ScrollViewer.
                        innerDivStyle.minWidth = "100%"; // This fixes an issue where the background of the "InputProject" test project is correct only at the top of the page.
                    }
                }
                else
                {
                    innerDivStyle.width = "100%";
                }
            }
        }

#if OPENSILVER
        private void INTERNAL_ApplyVerticalSettings(
            ScrollBarVisibility verticalScrollBarVisibility,
            INTERNAL_HtmlDomStyleReference outerDivStyle,
            INTERNAL_HtmlDomStyleReference innerDivStyle)
#elif BRIDGE
        private void INTERNAL_ApplyVerticalSettings(
            ScrollBarVisibility verticalScrollBarVisibility,
            dynamic outerDivStyle,
            dynamic innerDivStyle)
#endif
        {
            // if it's under customlayout, it works with Measure & Arrange.
            if (this.IsCustomLayoutRoot || this.IsUnderCustomLayout)
                return;

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
            {
                UpdateDomHorizontalOffset(INTERNAL_OuterDomElement);
            }

            SetScrollOffset(Orientation.Horizontal, offset);
        }

        /// <summary>
        /// If the ScrollViewer is Visible, sets the scrollLeft property to the value held in _horizontalOffset.
        /// If the ScrollViewer is Collapsed, it will set _isHorizontalOffsetInvalid to true,
        /// so that we know the current value of this.INTERNAL_OuterDomElement.scrollLeft is invalid and needs to be set when changing the visibility.
        /// </summary>
        /// <param name="div">The dom element on which to set the scrollLeft (normally the ScrollViewer's OuterDomElement)</param>
        internal void UpdateDomHorizontalOffset(object div)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                _isHorizontalOffsetInvalid = true;
            }
            else if (div != null) //Note: this can happen when we remove the ScrollViewer from the visual tree and one of its parents had its Visibility Collapsed.
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(div, "scrollLeft", _horizontalOffset);
                _isHorizontalOffsetInvalid = false;
            }
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
            {
                UpdateDomVerticalOffset(INTERNAL_OuterDomElement);
            }

            SetScrollOffset(Orientation.Vertical, offset);
        }

        /// <summary>
        /// If the ScrollViewer is Visible, sets the scrollTop property to the value held in _verticalOffset.
        /// If the ScrollViewer is Collapsed, it will set _isVerticalOffsetInvalid to true,
        /// so that we know the current value of this.INTERNAL_OuterDomElement.scrollTop is invalid and needs to be set when changing the visibility.
        /// </summary>
        /// <param name="div">The dom element on which to set the scrollTop (normally the ScrollViewer's OuterDomElement)</param>
        internal void UpdateDomVerticalOffset(object div)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                _isVerticalOffsetInvalid = true;
            }
            else if (div != null) //Note: this can happen when we remove the ScrollViewer from the visual tree and one of its parents had its Visibility Collapsed.
            {
                INTERNAL_HtmlDomManager.SetDomElementProperty(div, "scrollTop", _verticalOffset);
                _isVerticalOffsetInvalid = false;
            }
        }

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
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
        public static void SetVerticalScrollBarVisibility(DependencyObject element, ScrollBarVisibility verticalScrollBarVisibility)
        {
            element.SetValue(VerticalScrollBarVisibilityProperty, verticalScrollBarVisibility);
        }
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

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            if (!this.IsCustomLayoutRoot && !this.IsUnderCustomLayout)
                return;

            ElementScrollContentPresenter = GetTemplateChild(ElementScrollContentPresenterName) as ScrollContentPresenter;
            ElementHorizontalScrollBar = GetTemplateChild(ElementHorizontalScrollBarName) as ScrollBar;
            ElementVerticalScrollBar = GetTemplateChild(ElementVerticalScrollBarName) as ScrollBar;

            if (null != ElementHorizontalScrollBar)
            {
                ElementHorizontalScrollBar.Scroll += delegate (object sender, ScrollEventArgs e) { HandleScroll(Orientation.Horizontal, e); };
            }
            if (null != ElementVerticalScrollBar)
            {
                ElementVerticalScrollBar.Scroll += delegate (object sender, ScrollEventArgs e) { HandleScroll(Orientation.Vertical, e); };
            }
            UpdateScrollbarVisibility();
        }

        void SetScrollOffset(Orientation orientation, double value)
        {
            if (ScrollInfo != null)
            {
                double scrollable = (orientation == Orientation.Horizontal) ? ScrollableWidth : ScrollableHeight;
                double clamped = Math.Max(value, 0);

                clamped = Math.Min(scrollable, clamped);

                // Update ScrollContentPresenter 
                if (orientation == Orientation.Horizontal)
                    ScrollInfo.SetHorizontalOffset(clamped);
                else
                    ScrollInfo.SetVerticalOffset(clamped);

                UpdateScrollBar(orientation, clamped);
            }
        }
        /// <summary> 
        /// Handles the ScrollBar.Scroll event and updates the UI.
        /// </summary>
        /// <param name="orientation">Orientation of the ScrollBar.</param> 
        /// <param name="e">A ScrollEventArgs that contains the event data.</param> 
        private void HandleScroll(Orientation orientation, ScrollEventArgs e)
        {
            if (ScrollInfo != null)
            {
                bool horizontal = orientation == Orientation.Horizontal;

                // Calculate new offset 
                switch (e.ScrollEventType)
                {
                    case ScrollEventType.ThumbPosition:
                    case ScrollEventType.ThumbTrack:
                        SetScrollOffset(orientation, e.NewValue);
                        break;
                    case ScrollEventType.LargeDecrement:
                        if (horizontal)
                            ScrollInfo.PageLeft();
                        else
                            ScrollInfo.PageUp();
                        break;
                    case ScrollEventType.LargeIncrement:
                        if (horizontal)
                            ScrollInfo.PageRight();
                        else
                            ScrollInfo.PageDown();
                        break;
                    case ScrollEventType.SmallDecrement:
                        if (horizontal)
                            ScrollInfo.LineLeft();
                        else
                            ScrollInfo.LineUp();
                        break;
                    case ScrollEventType.SmallIncrement:
                        if (horizontal)
                            ScrollInfo.LineRight();
                        else
                            ScrollInfo.LineDown();
                        break;
                    case ScrollEventType.First:
                        SetScrollOffset(orientation, double.MinValue);
                        break;
                    case ScrollEventType.Last:
                        SetScrollOffset(orientation, double.MaxValue);
                        break;
                }
            }
        }

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
            if (ScrollInfo != null)
            {
                // ScrollBar visibility has to be updated before ViewportHeight/Width, because
                // that will trigger ScrollBar control sizing which depends on ScrollBar ActualWidth/Height
                UpdateScrollbarVisibility();

                ExtentHeight = ScrollInfo.ExtentHeight;
                ExtentWidth = ScrollInfo.ExtentWidth;
                ViewportHeight = ScrollInfo.ViewportHeight;
                ViewportWidth = ScrollInfo.ViewportWidth;
                UpdateScrollBar(Orientation.Horizontal, ScrollInfo.HorizontalOffset);
                UpdateScrollBar(Orientation.Vertical, ScrollInfo.VerticalOffset);
            }

            if (Math.Max(0, ExtentHeight - ViewportHeight) != ScrollableHeight)
            {
                ScrollableHeight = Math.Max(0, ExtentHeight - ViewportHeight);
                InvalidateMeasure();
            }
            if (Math.Max(0, ExtentWidth - ViewportWidth) != ScrollableWidth)
            {
                ScrollableWidth = Math.Max(0, ExtentWidth - ViewportWidth);
                InvalidateMeasure();
            }
        }

        void UpdateScrollbarVisibility()
        {
            // Update horizontal ScrollBar 
            Visibility horizontalVisibility;
            switch (HorizontalScrollBarVisibility)
            {
                case ScrollBarVisibility.Visible:
                    horizontalVisibility = Visibility.Visible;
                    break;
                case ScrollBarVisibility.Disabled:
                case ScrollBarVisibility.Hidden:
                    horizontalVisibility = Visibility.Collapsed;
                    break;
                default:  // Avoids compiler warning about uninitialized variable
                case ScrollBarVisibility.Auto:
                    horizontalVisibility = ScrollInfo == null || ScrollInfo.ExtentWidth <= ScrollInfo.ViewportWidth ? Visibility.Collapsed : Visibility.Visible;
                    break;
            }

            if (horizontalVisibility != ComputedHorizontalScrollBarVisibility)
            {
                ComputedHorizontalScrollBarVisibility = horizontalVisibility;
                InvalidateMeasure();
            }
            // Update vertical ScrollBar
            Visibility verticalVisibility;
            switch (VerticalScrollBarVisibility)
            {
                case ScrollBarVisibility.Visible:
                    verticalVisibility = Visibility.Visible;
                    break;
                case ScrollBarVisibility.Disabled:
                case ScrollBarVisibility.Hidden:
                    verticalVisibility = Visibility.Collapsed;
                    break;
                default:  // Avoids compiler warning about uninitialized variable
                case ScrollBarVisibility.Auto:
                    verticalVisibility = ScrollInfo == null || ScrollInfo.ExtentHeight <= ScrollInfo.ViewportHeight ? Visibility.Collapsed : Visibility.Visible;
                    break;
            }

            if (verticalVisibility != ComputedVerticalScrollBarVisibility)
            {
                ComputedVerticalScrollBarVisibility = verticalVisibility;
                InvalidateMeasure();
            }
        }
        void UpdateScrollBar(Orientation orientation, double value)
        {
            try
            {
                // Update relevant ScrollBar
                if (orientation == Orientation.Horizontal)
                {
                    HorizontalOffset = value;
                    if (ElementHorizontalScrollBar != null)
                    {
                        ElementHorizontalScrollBar.Value = value;
                    }
                }
                else
                {
                    VerticalOffset = value;
                    if (ElementVerticalScrollBar != null)
                    {
                        ElementVerticalScrollBar.Value = value;
                    }
                }
            }
            finally
            {

            }
        }

        private double ScrollBarWidth
        {
            get
            {
                return 20;  // Default scrollbar width
            }
        }

#if MIGRATION
        protected override void OnMouseWheel(MouseWheelEventArgs e)
#else
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseWheel(e);
#else
            base.OnPointerWheelChanged(e);
#endif

            if (!e.Handled && ScrollInfo != null)
            {
#if MIGRATION
                if (e.Delta < 0)
#else
                if (e.GetCurrentPoint(null).Properties.MouseWheelDelta < 0)
#endif
                {
                    ScrollInfo.MouseWheelDown();
                }
#if MIGRATION
                else if (0 < e.Delta)
#else
                else if (0 < e.GetCurrentPoint(null).Properties.MouseWheelDelta)
#endif
                {
                    ScrollInfo.MouseWheelUp();
                }

                e.Handled = true;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new ScrollViewerAutomationPeer(this);
    }
}
