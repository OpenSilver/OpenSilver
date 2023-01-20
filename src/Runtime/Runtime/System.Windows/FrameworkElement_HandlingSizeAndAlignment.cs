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
using System.ComponentModel;
using System.Globalization;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides a framework of common APIs for objects that participate in UI and
    /// programmatic layout. FrameworkElement also defines APIs related to data binding,
    /// object tree, and object lifetime feature areas.
    /// </summary>
    public abstract partial class FrameworkElement : UIElement
    {
        //static bool _theWarningAboutMarginsHasAlreadyBeenDisplayed = false;

        //--------------------------------------
        // Note: this is a "partial" class. This file handles anything related to Size and Alignment. Please refer to the other file for the rest of the FrameworkElement implementation.
        //--------------------------------------

        protected virtual void OnAfterApplyHorizontalAlignmentAndWidth()
        {
        }

        protected virtual void OnAfterApplyVerticalAlignmentAndWidth()
        {
        }

        internal static void INTERNAL_InitializeOuterDomElementWidthAndHeight(FrameworkElement element, object outerDomElement)
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif

            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(outerDomElement);
            if (element.IsUnderCustomLayout)
            {
                INTERNAL_HtmlDomManager.SetPosition(style, element.RenderedVisualBounds, false, true, true);
            }
            else
            {
                // Height:
                if (!double.IsNaN(element.Height))
                    style.height = element.Height.ToInvariantString() + "px";
                else if (element.VerticalAlignment == VerticalAlignment.Stretch && !(element.INTERNAL_VisualParent is Canvas) && !(element is CheckBox))
                    style.height = "100%";
                else
                    style.height = "auto";

                // Width:
                if (!double.IsNaN(element.Width))
                    style.width = element.Width.ToInvariantString() + "px";
                else if (element.HorizontalAlignment == HorizontalAlignment.Stretch && !(element.INTERNAL_VisualParent is Canvas) && !(element is CheckBox))
                    style.width = "100%";
                else
                    style.width = "auto";
            }
#if PERFSTAT
            Performance.Counter("Size/Alignment: INTERNAL_InitializeOuterDomElementWidthAndHeight", t0);
#endif
        }

        /// <summary>
        /// Gets or sets the Auto Width to the root of CustomLayout
        /// </summary>
        public bool? IsAutoWidthOnCustomLayout
        {
            get { return (bool?)GetValue(IsAutoWidthOnCustomLayoutProperty); }
            set { SetValue(IsAutoWidthOnCustomLayoutProperty, value); }
        }

        public static readonly DependencyProperty IsAutoWidthOnCustomLayoutProperty =
            DependencyProperty.Register(
                nameof(IsAutoWidthOnCustomLayout),
                typeof(bool?),
                typeof(FrameworkElement),
                new PropertyMetadata((object)null));

        internal bool IsAutoWidthOnCustomLayoutInternal
        {
            get
            {
                if (IsAutoWidthOnCustomLayout.HasValue)
                {
                    return IsAutoWidthOnCustomLayout.Value;
                }

                if (VisualTreeHelper.GetParent(this) is FrameworkElement parent)
                {
                    return parent.CheckIsAutoWidth(this);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the Auto Height to the root of CustomLayout
        /// </summary>
        public bool? IsAutoHeightOnCustomLayout
        {
            get { return (bool?)GetValue(IsAutoHeightOnCustomLayoutProperty); }
            set { SetValue(IsAutoHeightOnCustomLayoutProperty, value); }
        }

        public static readonly DependencyProperty IsAutoHeightOnCustomLayoutProperty =
            DependencyProperty.Register(
                nameof(IsAutoHeightOnCustomLayout),
                typeof(bool?),
                typeof(FrameworkElement),
                new PropertyMetadata((object)null));

        internal bool IsAutoHeightOnCustomLayoutInternal
        {
            get
            {
                if (IsAutoHeightOnCustomLayout.HasValue)
                {
                    return IsAutoHeightOnCustomLayout.Value;
                }

                if (VisualTreeHelper.GetParent(this) is FrameworkElement parent)
                {
                    return parent.CheckIsAutoHeight(this);
                }

                return false;
            }
        }

        /// <summary>
        /// Enable or disable measure/arrange layout system in a sub part
        /// </summary>
        public bool CustomLayout
        {
            get { return (bool)GetValue(CustomLayoutProperty); }
            set { SetValue(CustomLayoutProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.CustomLayout"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty CustomLayoutProperty =
            DependencyProperty.Register(
                nameof(CustomLayout),
                typeof(bool),
                typeof(FrameworkElement),
                new PropertyMetadata(false, CustomLayout_Changed));

        private static void CustomLayout_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if ((bool)e.NewValue && fe.IsCustomLayoutRoot)
                fe.LayoutRootSizeChanged += Element_SizeChanged;
            else
                fe.LayoutRootSizeChanged -= Element_SizeChanged;
        }

        private static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;

            if (fe.IsCustomLayoutRoot == false)
                return;

#if OPENSILVER
            if (OpenSilver.Interop.IsRunningInTheSimulator_WorkAround)
#elif BRIDGE
            if (OpenSilver.Interop.IsRunningInTheSimulator)
#endif
            {
                double width = Math.Max(0, e.NewSize.Width - fe.Margin.Left - fe.Margin.Right);
                double height = Math.Max(0, e.NewSize.Height - fe.Margin.Top - fe.Margin.Bottom);

                fe.UpdateCustomLayout(new Size(width, height));
            }
            else
            {
                fe.UpdateCustomLayout(e.NewSize);
            }
        }

        #region Height property

        /// <summary>
        /// Gets or sets the suggested height of a FrameworkElement.
        /// </summary>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.Height"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => (value is double) ?
                            !double.IsNaN((double)value) ? ((double)value).ToInvariantString() + "px" : "auto" :
                            throw new InvalidOperationException(
                                string.Format("Error when trying to set FrameworkElement.Height: expected double, got '{0}'.",
                                    value.GetType().FullName)),
                        CallbackMethod = Height_Changed,
                        UIElement = (UIElement)instance,
                        Name = new List<string> { "height" },
                        ApplyAlsoWhenThereIsAControlTemplate = true
                    }
                });

        private static void Height_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;

#if !PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS
            // We need to update vertical alignment because of the "Stretch" 
            // case which depends on whether the Height is set. It also makes 
            // the code simpler. //todo-performance: call only the relevant 
            // code in "INTERNAL_ApplyVerticalAlignmentAndHeight" not the 
            // whole method?
            INTERNAL_ApplyVerticalAlignmentAndHeight(frameworkElement, frameworkElement.VerticalAlignment);
#else
            RefreshHeight(frameworkElement);
#endif
            if (!double.IsNaN(frameworkElement.Width) && !double.IsNaN(frameworkElement.Height))
                frameworkElement.HandleSizeChanged(Size.Empty);
        }

#if PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS
        internal static void RefreshHeight(FrameworkElement frameworkElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(frameworkElement))
            {
                if (frameworkElement.Visibility != Visibility.Collapsed)
                {
                    var styleOfDomElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement);
                    var boxSizingElement = frameworkElement.INTERNAL_AdditionalOutsideDivForMargins;
                    var styleOfBoxSizingElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(boxSizingElement);
                    if (!double.IsNaN(frameworkElement.Height))
                    {
                        styleOfBoxSizingElement.height = "";
                        styleOfDomElement.height = frameworkElement.Height + "px";
                    }
                    else if (frameworkElement.VerticalAlignment == VerticalAlignment.Stretch && !(frameworkElement.INTERNAL_VisualParent is Canvas) && !(frameworkElement is CheckBox))
                    {
                        styleOfDomElement.height = "100%";
                        styleOfBoxSizingElement.height = "100%";
                    }
                    else
                    {
                        styleOfBoxSizingElement.height = "";
                        styleOfDomElement.height = "auto";

                    }
                }
            }

            //NOTE: we don't need to refresh the frameworkElement here for the "Stretch" case like in the Width_Changed method because it will already vertically center the element.
            //todo: we still need to make the default alignment depend on the type of the element (it is sometimes by default centered, sometimes top...)
        }
#endif

        #endregion


        #region Width property

        /// <summary>
        /// Gets or sets the width of a FrameworkElement.
        /// </summary>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => (value is double) ?
                            !double.IsNaN((double)value) ? ((double)value).ToInvariantString() + "px" : "auto" :
                            throw new InvalidOperationException(
                                string.Format("Error when trying to set FrameworkElement.Width: expected double, got '{0}'.",
                                    value.GetType().FullName)),
                        CallbackMethod = Width_Changed,
                        UIElement = (UIElement)instance,
                        Name = new List<string> { "width" },
                        ApplyAlsoWhenThereIsAControlTemplate = true
                    }
                });

        internal static void Width_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;

#if !PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS
            // We need to update horizontal alignment because of the "Stretch"
            // case which depends on whether the Width is set. It also makes 
            // the code simpler. //todo-performance: call only the relevant 
            // code in "INTERNAL_ApplyHorizontalAlignmentAndWidth" not the 
            //whole method?
            INTERNAL_ApplyHorizontalAlignmentAndWidth(frameworkElement, frameworkElement.HorizontalAlignment);
#else
            RefreshWidth(frameworkElement);
#endif
            if (!double.IsNaN(frameworkElement.Width) && !double.IsNaN(frameworkElement.Height))
                frameworkElement.HandleSizeChanged(Size.Empty);
        }

#if PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS
        internal static void RefreshWidth(FrameworkElement frameworkElement)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(frameworkElement))
            {
                if (frameworkElement.Visibility != Visibility.Collapsed)
                {
                    var outerDomElementStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement);
                    var boxSizingElement = frameworkElement.INTERNAL_AdditionalOutsideDivForMargins;
                    var styleOfBoxSizingElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(boxSizingElement);
                    if (!double.IsNaN(frameworkElement.Width))
                    {
                        styleOfBoxSizingElement.width = "";
                        outerDomElementStyle.width = frameworkElement.Width + "px";
                    }
                    else if (frameworkElement.HorizontalAlignment == HorizontalAlignment.Stretch && !(frameworkElement.INTERNAL_VisualParent is Canvas))
                    {
                        styleOfBoxSizingElement.width = "100%";
                        outerDomElementStyle.width = "100%";
                    }
                    else
                    {
                        styleOfBoxSizingElement.width = "";
                        outerDomElementStyle.width = "auto";
                    }
                }
            }

            // We need to update horizontal alignment because of the "Stretch" case which depends on whether the Width is set. //todo-performance: call only the relevant code in "RefreshHorizontalAlignment" not the whole method?
            RefreshHorizontalAlignment(frameworkElement, frameworkElement.HorizontalAlignment);
        }
#endif

        #endregion


        #region HorizontalAlignment and Width handling

        /// <summary>
        /// Gets or sets the horizontal alignment characteristics that are applied to
        /// a FrameworkElement when it is composed in a layout parent, such as a panel
        /// or items control.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.HorizontalAlignment"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalAlignment),
                typeof(HorizontalAlignment),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, HorizontalAlignment_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void HorizontalAlignment_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            HorizontalAlignment newHorizontalAlignment = (HorizontalAlignment)e.NewValue;
            INTERNAL_ApplyHorizontalAlignmentAndWidth(frameworkElement, newHorizontalAlignment);
        }

        internal static void INTERNAL_ApplyHorizontalAlignmentAndWidth(FrameworkElement fe, HorizontalAlignment newHorizontalAlignment)
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif
            if (fe.IsUnderCustomLayout)
                return;

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe)
                && fe.Visibility != Visibility.Collapsed)
            {
                // Gain access to the outer style:
                var styleOfOuterDomElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(fe);

                //We check if the element is the direct child of a ViewBox, in which case alignment has no meaning:
                UIElement currentParent = fe.INTERNAL_VisualParent as UIElement;
                //the test below is basically: frameworkElement.VisualParent.VisualParent.VisualParent is ViewBox
                bool isParentAViewBox =
                    currentParent != null
                    && ((currentParent = currentParent.INTERNAL_VisualParent as UIElement) != null)
                    && currentParent.INTERNAL_VisualParent as Viewbox != null; //todo: this test is unlikely to work with a custom Template on the ViewBox, use frameworkElement.LogicalParent (or something like that) once the logical tree branch will be integrated)

                // If the element is inside a Canvas, we ignore alignment and only apply the Width/Height:
                if (fe.INTERNAL_VisualParent is Canvas || isParentAViewBox) //todo: replace the second part of this test with something meaning "logical parent is ViewBox" instead once we will have the logical tree (we cannot do that yet since we cannot access the ViewBox from frameworkElement).
                {
                    styleOfOuterDomElement.width = !double.IsNaN(fe.Width) ? 
                        fe.Width.ToInvariantString() + "px" : 
                        "auto";
                }
                else // Otherwise we handle both alignment and Width/Height:
                {
#if !PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS

                    //-----------------------------
                    // Gain access to the styles:
                    //-----------------------------

                    var wrapperElement = fe.INTERNAL_AdditionalOutsideDivForMargins;
                    var styleOfWrapperElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(wrapperElement);

                    //-----------------------------
                    // Handle special cases:
                    //-----------------------------

                    if (fe is ChildWindow)
                    {
                        //we force the Stretch behaviour on ChildWindow so that the Gray background thing can be on the whole window.
                        styleOfWrapperElement.width = "100%";
                        styleOfOuterDomElement.marginLeft = "0px"; // Default value
                        styleOfOuterDomElement.marginRight = "0px"; // Default value
                        styleOfOuterDomElement.display = "block"; // Default value
                        styleOfOuterDomElement.width = "100%";
                        styleOfOuterDomElement.maxWidth = "none";
                        styleOfOuterDomElement.maxHeight = "none";
                        return;
                    }

                    // If the alignment is "Stretch" AND a size in pixels is specified, the behavior is similar to "Center".
                    // Also, if the alignment is "Stretch" AND the element is an Image with Stretch = None this same case applies since it has a fixed size too:
                    if (newHorizontalAlignment == HorizontalAlignment.Stretch
                        && (!double.IsNaN(fe.Width) || (fe is Image && ((Image)fe).Stretch == Media.Stretch.None)))
                    {
                        newHorizontalAlignment = HorizontalAlignment.Center;
                    }

                    //If the element is an Image and it has Stretch = Uniform and no size, the behavior is similar to "Stretch":
                    if (fe is Image
                        && double.IsNaN(fe.Width)
                        && double.IsNaN(fe.Height)
                        && ((Image)fe).Stretch != Media.Stretch.None)
                    {
                        newHorizontalAlignment = HorizontalAlignment.Stretch;
                    }

                    //-----------------------------
                    // Apply CSS alignment and size:
                    //-----------------------------

                    switch (newHorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.justifyContent = "start";
                            styleOfOuterDomElement.width = !double.IsNaN(fe.Width) ? fe.Width.ToInvariantString() + "px" : "auto";
                            break;
                        case HorizontalAlignment.Center:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.justifyContent = "center";
                            styleOfOuterDomElement.width = !double.IsNaN(fe.Width) ? fe.Width.ToInvariantString() + "px" : "auto";
                            break;
                        case HorizontalAlignment.Right:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.justifyContent = "end";
                            styleOfOuterDomElement.width = !double.IsNaN(fe.Width) ? fe.Width.ToInvariantString() + "px" : "auto";
                            break;
                        case HorizontalAlignment.Stretch:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.justifyContent = "stretch";
                            styleOfOuterDomElement.width = "100%";
                            break;
                        default:
                            break;
                    }
                }

                //-----------------------------
                // Handle the "Overflow" CSS property:
                //-----------------------------

                /*
                 
                // COMMENTED ON 2016.09.02 because it prevents properly displaying child elements with NEGATIVE MARGINS. To reproduce: put a border with negative margins inside another border.
                 
                if (!(frameworkElement is ScrollViewer)) //Note: The ScrollViewer handles the "overflow" property by itself.
                {
                    // We always display the portions of the child exceeding the edges, unless the element has a fixed size in pixels AND it is not a canvas:
                    if (!double.IsNaN(frameworkElement.Width) && !(frameworkElement is Canvas))
                        styleOfOuterDomElement.overflowX = "hidden"; //Note: This value means to crop the portion of the child exceeding the edges.
                    else
                        styleOfOuterDomElement.overflowX = ""; //Note: the default value is "visible"
                }
                 */

                //-----------------------------
                // Call code from derived class if any:
                //-----------------------------

                fe.OnAfterApplyHorizontalAlignmentAndWidth();
#else

                var outerDomElementStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement);
                var boxSizingElement = frameworkElement.INTERNAL_AdditionalOutsideDivForMargins;
                var styleOfBoxSizingElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(boxSizingElement);

                string marginLeft = "0px";
                string marginRight = "0px";
                if ((frameworkElement.INTERNAL_VisualParent is Canvas)) //if the parent is a canvas, we ignore this property
                {
                    if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                    {
                        if (!double.IsNaN(frameworkElement.Width) && !(frameworkElement is Canvas))
                        {
                            styleOfBoxSizingElement.overflowX = "hidden";
                        }
                        else
                        {
                            styleOfBoxSizingElement.overflowX = "display";
                        }
                    }
                    //marginRight = "auto";

                    //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).marginLeft = marginLeft;
                    //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).marginRight = marginRight;
                }
                else
                {
                    if (newHorizontalAlignment == HorizontalAlignment.Left)
                    {
                        //todo: Add this as the parent of the element if it is not already there:
                        //<div style="display:table; position: relative;"> //(the elements are by default put on the left but we still add it so that it is here)
                        if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                        {
                            if (!double.IsNaN(frameworkElement.Width) && !(frameworkElement is Canvas))
                            {
                                styleOfBoxSizingElement.overflowX = "hidden";
                            }
                            else
                            {
                                styleOfBoxSizingElement.overflowX = "display";
                            }
                        }
                        marginRight = "auto";
                    }
                    else if (newHorizontalAlignment == HorizontalAlignment.Center
                        || (!double.IsNaN(frameworkElement.Width) && newHorizontalAlignment == HorizontalAlignment.Stretch)) // Note: this second test is because elements that have a fixed size but a "Stretch" alignment behave as if they were centered.
                    {
                        //todo: Add this as the parent of the element if it is not already there:
                        //<div style="display:table; position: relative; margin-right: auto; margin-left:auto;">
                        if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                        {
                            if (!double.IsNaN(frameworkElement.Width) && !(frameworkElement is Canvas))
                            {
                                styleOfBoxSizingElement.overflowX = "hidden";
                            }
                            else
                            {
                                styleOfBoxSizingElement.overflowX = "display";
                            }
                        }
                        marginLeft = "auto";
                        marginRight = "auto";
                    }
                    else if (newHorizontalAlignment == HorizontalAlignment.Right)
                    {
                        //todo: Add this as the parent of the element if it is not already there:
                        //<div style="display:table; position: relative; margin-left:auto;">
                        if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                        {
                            if (!double.IsNaN(frameworkElement.Width) && !(frameworkElement is Canvas))
                            {
                                styleOfBoxSizingElement.overflowX = "hidden";
                            }
                            else
                            {
                                styleOfBoxSizingElement.overflowX = "display";
                            }
                        }
                        marginLeft = "auto";
                    }
                    else if (newHorizontalAlignment == HorizontalAlignment.Stretch)
                    {
                        //todo: Add this as the parent of the element if it is not already there:
                        //<div style="display:table; position: relative; margin-right: 0px; margin-left:0px;">

                        if (frameworkElement is ScrollViewer) //the ScrollViewer handles his overflows by himself
                        {
                            if (double.IsNaN(frameworkElement.Width))
                            {
                                outerDomElementStyle.width = "100%";
                            }
                        }
                        else
                        {
                            if (!double.IsNaN(frameworkElement.Width))
                            {
                                if (frameworkElement is Canvas)
                                {
                                    styleOfBoxSizingElement.overflowX = "display";
                                }
                                else
                                {
                                    styleOfBoxSizingElement.overflowX = "hidden";
                                }
                                marginLeft = "auto";
                                marginRight = "auto";
                            }
                            else
                            {
                                styleOfBoxSizingElement.overflowX = "display";
                                outerDomElementStyle.width = "100%";
                            }
                        }
                    }

                    //the "parent is canvas" case is dealt with before the switch
                    outerDomElementStyle.position = "relative";

                    if (newHorizontalAlignment == HorizontalAlignment.Stretch || frameworkElement is ScrollViewer)
                        outerDomElementStyle.display = "block";
                    else
                        outerDomElementStyle.display = "table";


                    outerDomElementStyle.marginLeft = marginLeft;
                    outerDomElementStyle.marginRight = marginRight;
                }
#endif

#if PERFSTAT
                Performance.Counter("Size/Alignment: INTERNAL_ApplyHorizontalAlignmentAndWidth", t0);
#endif
            }
        }

        #endregion


        #region VerticalAlignment and Height handling

        /// <summary>
        /// Gets or sets the vertical alignment characteristics that are applied to a
        /// FrameworkElement when it is composed in a parent object such as a panel or
        /// items control.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.VerticalAlignment"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalAlignment),
                typeof(VerticalAlignment),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, VerticalAlignment_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void VerticalAlignment_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            VerticalAlignment newVerticalAlignment = (VerticalAlignment)e.NewValue;
            INTERNAL_ApplyVerticalAlignmentAndHeight(frameworkElement, newVerticalAlignment);
        }

        internal static void INTERNAL_ApplyVerticalAlignmentAndHeight(FrameworkElement fe, VerticalAlignment newVerticalAlignment)
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif
            if (fe.IsUnderCustomLayout)
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe)
                    && fe.Visibility != Visibility.Collapsed
                    && !double.IsNaN(fe.Height))
                {
                    var styleOfOuterDomElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(fe);
                    styleOfOuterDomElement.height = fe.Height.ToInvariantString() + "px";
                }
                return;
            }

            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe)
                && fe.Visibility != Visibility.Collapsed)
            {
                // Gain access to the outer style:
                var styleOfOuterDomElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(fe);

                //We check if the element is the direct child of a ViewBox, in which case alignment has no meaning:
                UIElement currentParent = fe.INTERNAL_VisualParent as UIElement;
                //the test below is basically: frameworkElement.VisualParent.VisualParent.VisualParent is ViewBox
                bool isParentAViewBox =
                    currentParent != null
                    && ((currentParent = currentParent.INTERNAL_VisualParent as UIElement) != null)
                    && currentParent.INTERNAL_VisualParent as Viewbox != null; //todo: this test is unlikely to work with a custom Template on the ViewBox, use frameworkElement.LogicalParent (or something like that) once the logical tree branch will be integrated)


                // If the element is inside a Canvas, we ignore alignment and only apply the Width/Height:
                if (fe.INTERNAL_VisualParent is Canvas || isParentAViewBox)
                {
                    styleOfOuterDomElement.height = !double.IsNaN(fe.Height) ? fe.Height.ToInvariantString() + "px" : "auto";
                }
                else // Otherwise we handle both alignment and Width/Height:
                {
#if !PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS

                    //-----------------------------
                    // Gain access to the styles:
                    //-----------------------------

                    var wrapperElement = fe.INTERNAL_AdditionalOutsideDivForMargins;
                    var styleOfWrapperElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(wrapperElement);

                    //-----------------------------
                    // Handle special cases:
                    //-----------------------------

                    if (fe is ChildWindow)
                    {
                        //we force the Stretch behaviour on ChildWindow so that the Gray background thing can be on the whole window.
                        styleOfWrapperElement.verticalAlign = "middle"; // This might be useless.
                        styleOfOuterDomElement.height = "100%";
                        return;
                    }


                    // If the alignment is "Stretch" AND a size in pixels is specified, the behavior is similar to "Center".
                    // Also, if the alignment is "Stretch" AND the element is an Image with Stretch = None this same case applies since it has a fixed size too:
                    if (newVerticalAlignment == VerticalAlignment.Stretch
                        && (!double.IsNaN(fe.Height) || (fe is Image && ((Image)fe).Stretch == Media.Stretch.None)))
                    {
                        newVerticalAlignment = VerticalAlignment.Center;
                    }

                    //If the element is an Image and it has Stretch = Uniform and no size, the behavior is similar to "Stretch":
                    if (fe is Image
                        && double.IsNaN(fe.Width)
                        && double.IsNaN(fe.Height)
                        && ((Image)fe).Stretch != Media.Stretch.None)
                    {
                        newVerticalAlignment = VerticalAlignment.Stretch;
                    }

                    //-----------------------------
                    // Apply CSS alignment and size:
                    //-----------------------------

                    switch (newVerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.alignItems = "start";
                            styleOfOuterDomElement.height = !double.IsNaN(fe.Height) ? $"{fe.Height.ToInvariantString()}px" : "auto";
                            break;
                        case VerticalAlignment.Center:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.alignItems = "center";
                            styleOfOuterDomElement.height = !double.IsNaN(fe.Height) ? $"{fe.Height.ToInvariantString()}px" : "auto";
                            break;
                        case VerticalAlignment.Bottom:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.alignItems = "end";
                            styleOfOuterDomElement.height = !double.IsNaN(fe.Height) ? $"{fe.Height.ToInvariantString()}px" : "auto";
                            break;
                        case VerticalAlignment.Stretch:
                            styleOfWrapperElement.display = "flex";
                            styleOfWrapperElement.alignItems = "stretch";
                            styleOfOuterDomElement.height = "100%";
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                //-----------------------------
                // Call code from derived class if any:
                //-----------------------------

                fe.OnAfterApplyVerticalAlignmentAndWidth();
#else

                var boxSizingElement = frameworkElement.INTERNAL_AdditionalOutsideDivForMargins;
                var styleOfBoxSizingElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(boxSizingElement);
                string verticalAlignCssProperty = "middle";
                if ((frameworkElement.INTERNAL_VisualParent is Canvas)) //if the parent is a canvas, we consider that it always is top-left
                {
                    if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                    {
                        if (!double.IsNaN(frameworkElement.Height))
                        {
                            styleOfBoxSizingElement.overflowY = "hidden";
                            //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "hidden";
                        }
                        else
                        {
                            styleOfBoxSizingElement.overflowY = "display";
                            //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "display";
                        }
                    }
                    verticalAlignCssProperty = "top";

                    var immediateDomParent = INTERNAL_HtmlDomManager.GetParentDomElement(frameworkElement.INTERNAL_AdditionalOutsideDivForMargins);
                    var immediateDomParentStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(immediateDomParent);

                    //immediateDomParent.style.display = "table-cell"; //TESTTSET

                    immediateDomParentStyle.verticalAlign = verticalAlignCssProperty;
                }
                else
                {
                    if (newVerticalAlignment == VerticalAlignment.Top)
                    {
                        if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                        {
                            if (!double.IsNaN(frameworkElement.Height) && !(frameworkElement is Canvas))
                            {
                                styleOfBoxSizingElement.overflowY = "hidden";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "hidden";
                            }
                            else
                            {
                                styleOfBoxSizingElement.overflowY = "display";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "display";
                            }
                        }
                        verticalAlignCssProperty = "top";
                    }
                    else if (newVerticalAlignment == VerticalAlignment.Center
                        || (!double.IsNaN(frameworkElement.Height) && newVerticalAlignment == VerticalAlignment.Stretch)) // Note: this second test is because elements that have a fixed size but a "Stretch" alignment behave as if they were centered.
                    {
                        if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                        {
                            if (!double.IsNaN(frameworkElement.Height) && !(frameworkElement is Canvas))
                            {
                                styleOfBoxSizingElement.overflowY = "hidden";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "hidden";
                            }
                            else
                            {
                                styleOfBoxSizingElement.overflowY = "display";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "display";
                            }
                        }
                        verticalAlignCssProperty = "middle";
                    }
                    else if (newVerticalAlignment == VerticalAlignment.Bottom)
                    {
                        if (!(frameworkElement is ScrollViewer)) //the ScrollViewer handles his overflows by himself
                        {
                            if (!double.IsNaN(frameworkElement.Height) && !(frameworkElement is Canvas))
                            {
                                styleOfBoxSizingElement.overflowY = "hidden";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "hidden";
                            }
                            else
                            {
                                styleOfBoxSizingElement.overflowY = "display";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "display";
                            }
                        }
                        verticalAlignCssProperty = "bottom";
                    }
                    else if (newVerticalAlignment == VerticalAlignment.Stretch)
                    {
                        //todo
                        if (frameworkElement is ScrollViewer) //the ScrollViewer handles his overflows by himself
                        {
                            if (double.IsNaN(frameworkElement.Height))
                            {
                                styleOfBoxSizingElement.height = "100%";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).height = "100%";
                            }
                        }
                        else
                        {
                            if (!double.IsNaN(frameworkElement.Height)) //the ScrollViewer handles his overflows by himself
                            {
                                if (frameworkElement is Canvas)
                                {
                                    styleOfBoxSizingElement.overflowY = "display";
                                }
                                else
                                {
                                    styleOfBoxSizingElement.overflowY = "hidden";
                                }
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "hidden";
                            }
                            else if (!(frameworkElement is CheckBox))
                            {
                                styleOfBoxSizingElement.height = "100%";
                                styleOfBoxSizingElement.overflowY = "display";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).height = "100%";
                                //INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement).overflowY = "display";
                            }
                        }
                    }

                    // Set style of "immediate DOM parent": "position: relative; display: table; vertical-align: bottom;"
                    var immediateDomParent = INTERNAL_HtmlDomManager.GetParentDomElement(frameworkElement.INTERNAL_AdditionalOutsideDivForMargins);
                    if (immediateDomParent != null) //Note: it can be null if the element is the MainPage and the immediateDomParent is the ROOT control.
                    {
                        var immediateDomParentStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(immediateDomParent);

                        immediateDomParentStyle.position = "relative";
                        if (newVerticalAlignment == VerticalAlignment.Stretch)
                        {
                            immediateDomParentStyle.display = "inline-block";
                        }
                        else if (!(frameworkElement.INTERNAL_VisualParent is StackPanel && ((StackPanel)frameworkElement.INTERNAL_VisualParent).Orientation == Orientation.Vertical))
                        {
                            immediateDomParentStyle.display = "table-cell";
                        }
                        else
                        {
                            immediateDomParentStyle.display = "";
                        }
                        immediateDomParentStyle.verticalAlign = verticalAlignCssProperty;

                        // Set style of outerDIV of current element (unless the visual parent is a Canvas, which case is already dealt with before the switch): style="position: relative;":
                        var outerDomElementStyle = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement);
                        outerDomElementStyle.position = "relative";

                        var grandParent = INTERNAL_HtmlDomManager.GetParentDomElement(immediateDomParent);
                        if (grandParent != null) //Note: it can be null if the immediateDomParent is the MainPage and the grand parent is the ROOT control.
                        {
                            var grandParentStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(grandParent);
                            grandParentStyle.display = "table";
                        }
                    }

                    // OBSOLETE:
                    // it should be like this
                    //<something style="height: inherit; position: relative; display: table-cell; vertical-align: bottom;">
                    //NOTE: something is the parent element --it should be:
                    // - the first <td> element in the wrapper when we go from the outside of the wrapper to the inside
                    // - if no <td> element, the outer element of the wrapper for the child
                    // - if no wrapper: add a div around the child and apply this style to it.
                    //  <div style="position: relative;">
                    //      ...Element (example below)
                    //      <div style="display:table;background-color: rgb(255, 0, 255); margin-right:50px;">bottom right</div>
                    //  </div>
                    //</something>
                    //EXAMPLE WITH <td>
                    //<td style="padding: 0px; height: inherit; position: relative; display: table-cell; vertical-align: top">
                    //  <div style="width: 50px; height: 50px;position: relative;"> <!-- this is a canvas (child of the stackpanel) -->
                    //      <div style="background-color: rgb(200,0,200);width:20px;height:20px;  position: absolute"></div>
                    //      <div style="background-color: rgb(100,0,200);width:20px;height:20px;margin-left:20px;margin-right:auto;  position: absolute"></div>
                    //  </div> <!-- this is the end of the canvas -->
                    //</td>
                    //EXAMPLE WITH <div>
                    //<div style="width: inherit; height: inherit; position: relative; display: table-cell; vertical-align: bottom;">
                    //    <div style="display:table; position: relative; margin-right: 0px; margin-left:auto;"> ---this div is here to handle horizontal alignment
                    //        <div style="background-color: rgb(255, 0, 255);">bottom right</div> ---thi is the element
                    //    </div>
                    //</div>
                }
#endif

#if PERFSTAT
                Performance.Counter("Size/Alignment: INTERNAL_ApplyVerticalAlignmentAndHeight", t0);
#endif
            }
        }

        #endregion


        #region Margin

        /// <summary>
        /// Gets or sets the outer margin of a FrameworkElement.
        /// </summary>
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.Margin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(
                nameof(Margin),
                typeof(Thickness),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
                {
                    MethodToUpdateDom = Margin_MethodToUpdateDom,
                });

        internal static void Margin_MethodToUpdateDom(DependencyObject d, object newValue)
        {
            var fe = (FrameworkElement)d;            
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe) && !fe.IsUnderCustomLayout)
            {
                var margin = (Thickness)newValue;
                var styleOfBoxSizingElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(fe.INTERNAL_AdditionalOutsideDivForMargins);
                var styleOfOuterDomElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(fe.INTERNAL_OuterDomElement);
                if (margin.Left >= 0)
                {
                    styleOfBoxSizingElement.paddingLeft = $"{margin.Left.ToInvariantString()}px";
                    styleOfBoxSizingElement.marginLeft = string.Empty;
                }
                else
                {
                    styleOfBoxSizingElement.paddingLeft = string.Empty;
                    styleOfBoxSizingElement.marginLeft = $"{margin.Left.ToInvariantString()}px";
                }

                if (margin.Top >= 0)
                {
                    styleOfBoxSizingElement.paddingTop = $"{margin.Top.ToInvariantString()}px";
                    styleOfOuterDomElement.marginTop = string.Empty;
                }
                else
                {
                    styleOfBoxSizingElement.paddingTop = string.Empty;
                    styleOfOuterDomElement.marginTop = $"{margin.Top.ToInvariantString()}px";
                }

                if (margin.Right >= 0)
                {
                    styleOfBoxSizingElement.paddingRight = $"{margin.Right.ToInvariantString()}px";
                    styleOfBoxSizingElement.marginRight = string.Empty;
                }
                else
                {
                    styleOfBoxSizingElement.paddingRight = string.Empty;
                    styleOfBoxSizingElement.marginRight = $"{margin.Right.ToInvariantString()}px";
                    styleOfBoxSizingElement.width = "auto";
                }

                if (margin.Bottom >= 0)
                {
                    styleOfBoxSizingElement.paddingBottom = $"{margin.Bottom.ToInvariantString()}px";
                    styleOfOuterDomElement.marginBottom = string.Empty;
                }
                else
                {
                    styleOfBoxSizingElement.paddingBottom = string.Empty;
                    styleOfOuterDomElement.marginBottom = $"{margin.Bottom.ToInvariantString()}px";
                }
            }
        }

        #endregion


        #region MinHeight

        /// <summary>
        /// Gets or sets the minimum height constraint of a FrameworkElement.
        /// </summary>
        public double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.MinHeight"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                nameof(MinHeight),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, MinHeight_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MinHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe) && !fe.IsUnderCustomLayout)
            {
                double minHeight = (double)e.NewValue;
                var domElement = fe.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? fe.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElement);
                style.minHeight = !double.IsNaN(minHeight) && minHeight > 0 ? $"{minHeight.ToInvariantString()}px" : string.Empty;
            }
        }

        #endregion


        #region MinWidth

        /// <summary>
        /// Gets or sets the minimum width constraint of a FrameworkElement.
        /// </summary>
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.MinWidth"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                nameof(MinWidth),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, MinWidth_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MinWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe) && !fe.IsUnderCustomLayout)
            {
                double minWidth = (double)e.NewValue;
                var domElement = fe.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? fe.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElement);
                style.minWidth = !double.IsNaN(minWidth) && minWidth > 0 ? $"{minWidth.ToInvariantString()}px" : string.Empty;
            }
        }

        #endregion


        #region MaxHeight

        /// <summary>
        /// Gets or sets the maximum height constraint of a FrameworkElement.
        /// </summary>
        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.MaxHeight"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                nameof(MaxHeight),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, MaxHeight_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MaxHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe) && !fe.IsUnderCustomLayout)
            {
                double maxHeight = (double)e.NewValue;
                var domElement = fe.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? fe.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElement);
                // Commented because at the time of writing "IsInfinity" was not implemented in JSIL:
                //style.maxHeight = !double.IsInfinity(newValue) ? newValue.ToString() + "px" : "initial";
                style.maxHeight = (maxHeight != double.MaxValue) ? $"{maxHeight.ToInvariantString()}px" : string.Empty;
            }
        }

        #endregion


        #region MaxWidth

        /// <summary>
        /// Gets or sets the maximum width constraint of a FrameworkElement.
        /// </summary>
        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.MaxWidth"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, MaxWidth_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MaxWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(fe) && !fe.IsUnderCustomLayout)
            {
                double maxWidth = (double)e.NewValue;
                var domElement = fe.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? fe.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElement);
                // Commented because at the time of writing "IsInfinity" was not implemented in JSIL:
                //style.maxWidth = !double.IsInfinity(newValue) ? newValue.ToString() + "px" : "initial";
                style.maxWidth = (maxWidth != double.MaxValue) ? $"{maxWidth.ToInvariantString()}px" : string.Empty;
            }
        }

        #endregion


        #region ActualWidth / ActualHeight

        /// <summary>
        /// Gets the rendered width of a FrameworkElement. The FrameworkElement must be in the visual tree,
        /// otherwise this property will return 0.
        /// </summary>
        public double ActualWidth
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    if (IsCustomLayoutRoot || IsUnderCustomLayout)
                    {
                        return VisualBounds.Width;
                    }
                        
                    try
                    {
                        return INTERNAL_HtmlDomManager.GetDomElementAttributeInt32(INTERNAL_OuterDomElement, "offsetWidth");
                    }
                    catch
                    {
                        return 0d;
                    }
                }
                
                return 0d;
            }
        }

        /// <summary>
        /// Gets the rendered height of a FrameworkElement. The FrameworkElement must be in the visual tree,
        /// otherwise this property will return 0.
        /// </summary>
        public double ActualHeight
        {
            get
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && INTERNAL_OuterDomElement != null)
                {
                    if (IsCustomLayoutRoot || IsUnderCustomLayout)
                    {
                        return VisualBounds.Height;
                    }

                    try
                    {
                        return INTERNAL_HtmlDomManager.GetDomElementAttributeInt32(INTERNAL_OuterDomElement, "offsetHeight");
                    }
                    catch
                    {
                        return 0d;
                    }
                }
                
                return 0d;
            }
        }

        public static readonly DependencyProperty ActualWidthProperty =
            DependencyProperty.Register(
                nameof(ActualWidth),
                typeof(double),
                typeof(FrameworkElement),
                null);

        public static readonly DependencyProperty ActualHeightProperty =
            DependencyProperty.Register(
                nameof(ActualHeight),
                typeof(double),
                typeof(FrameworkElement),
                null);

        bool _isSubsribedToSizeChanged = false;
        internal void SubsribeToSizeChanged()
        {
            if (!_isSubsribedToSizeChanged)
            {
                SizeChanged += (s, e) =>
                {
                    if (((double)GetValue(ActualWidthProperty)) != e.NewSize.Width)
                    {
                        SetValue(ActualWidthProperty, e.NewSize.Width);
                    }

                    if (((double)GetValue(ActualHeightProperty)) != e.NewSize.Height)
                    {
                        SetValue(ActualHeightProperty, e.NewSize.Height);
                    }
                };
                _isSubsribedToSizeChanged = true;
            }
        }

        /// <summary>
        /// Gets the rendered size of a FrameworkElement.
        /// </summary>
        public Size ActualSize
        {
            get { return this.INTERNAL_GetActualWidthAndHeight(); }
        }

        /// <summary>
        /// Use this method for better performance in the Simulator compared to requesting the ActualWidth and ActualHeight separately.
        /// </summary>
        /// <returns>The actual size of the element.</returns>
        internal Size INTERNAL_GetActualWidthAndHeight()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && this.INTERNAL_OuterDomElement != null)
            {
                if (!double.IsNaN(this.Width) && !double.IsNaN(this.Height))
                    return new Size(this.Width, this.Height);
                try
                {
                    // Hack to improve the Simulator performance by making only one interop call rather than two:
                    string concatenated = OpenSilver.Interop.ExecuteJavaScriptString(
                        $"document.getActualWidthAndHeight({CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement)})");
                    int sepIndex = concatenated != null ? concatenated.IndexOf('|') : -1;
                    if (sepIndex > -1)
                    {
                        string actualWidthAsString = concatenated.Substring(0, sepIndex);
                        string actualHeightAsString = concatenated.Substring(sepIndex + 1);
                        double actualWidth = double.Parse(actualWidthAsString, CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                        double actualHeight = double.Parse(actualHeightAsString, CultureInfo.InvariantCulture); //todo: read note above
                        return new Size(actualWidth, actualHeight);
                    }
                    else
                    {
                        return new Size(0d, 0d);
                    }
                }
                catch
                {
                    return new Size(0d, 0d);
                }
            }
            
            return new Size(0d, 0d);
        }

        #endregion


        #region SizeChanged

        private Size _valueOfLastSizeChanged = new Size(0d, 0d);
        private List<SizeChangedEventHandler> _sizeChangedEventHandlers;
        private readonly IResizeObserverAdapter _resizeObserver = ResizeObserverFactory.Create();

        // Size changed event for the CustomLayout Root
        internal Size _valueOfLayoutRootLastSizeChanged = new Size(0d, 0d);
        internal List<SizeChangedEventHandler> _layoutRootSizeChangedEventHandlers;
        internal readonly IResizeObserverAdapter _layoutRootResizeObserver = ResizeObserverFactory.Create();

        private void HandleSizeChanged(Size currentSize)
        {
            if (this._sizeChangedEventHandlers != null
               && this._sizeChangedEventHandlers.Count > 0
               && INTERNAL_VisualTreeManager.IsElementInVisualTree(this)
               && this._isLoaded)
            {
                if (currentSize == Size.Empty)
                {
                    currentSize = this.INTERNAL_GetActualWidthAndHeight();
                }

                if (!Size.Equals(this._valueOfLastSizeChanged, currentSize))
                {
                    this._valueOfLastSizeChanged = currentSize;

                    SizeChangedEventArgs e = new SizeChangedEventArgs(currentSize);

                    // Raise the "SizeChanged" event of all the listeners:
                    for (int i = 0; i < this._sizeChangedEventHandlers.Count; i++)
                    {
                        this._sizeChangedEventHandlers[i](this, e);
                    }
                }
            }
        }

        internal void LayoutRootHandleSizeChanged(Size currentSize)
        {
            if (this._layoutRootSizeChangedEventHandlers != null
               && this._layoutRootSizeChangedEventHandlers.Count > 0
               && INTERNAL_VisualTreeManager.IsElementInVisualTree(this)
               && this._isLoaded)
            {
                if (currentSize == Size.Empty)
                {
                    currentSize = this.VisualBounds.Size;
                }

                if (!Size.Equals(this._valueOfLayoutRootLastSizeChanged, currentSize))
                {
                    this._valueOfLayoutRootLastSizeChanged = currentSize;

                    SizeChangedEventArgs e = new SizeChangedEventArgs(currentSize);

                    // Raise the "LayoutRootSizeChanged" event of all the listeners:
                    for (int i = 0; i < this._layoutRootSizeChangedEventHandlers.Count; i++)
                    {
                        this._layoutRootSizeChangedEventHandlers[i](this, e);
                    }
                }
            }
        }

        internal void INTERNAL_SizeChangedWhenAttachedToVisualTree() // Intended to be called by the "VisualTreeManager" when the FrameworkElement is attached to the visual tree.
        {
            // We reset the previous size value so that the SizeChanged event can be called (see the comment in "HandleSizeChanged"):
            _valueOfLastSizeChanged = Size.Empty;

            if (this.IsUnderCustomLayout == false)
            {
                if (this.IsCustomLayoutRoot == false)
                    HandleSizeChanged(Size.Empty);

                if (this._sizeChangedEventHandlers != null &&
                    this._sizeChangedEventHandlers.Count > 0 &&
                    !this._resizeObserver.IsObserved)
                {
                    if (double.IsNaN(this.Width) || double.IsNaN(this.Height))
                    {
                        _resizeObserver.Observe(this.INTERNAL_OuterDomElement, (Action<Size>)this.HandleSizeChanged);
                    }
                }
            }

            _valueOfLayoutRootLastSizeChanged = Size.Empty;
            if (this.IsCustomLayoutRoot)
            {
                if (this._layoutRootSizeChangedEventHandlers != null &&
                    this._layoutRootSizeChangedEventHandlers.Count > 0 &&
                    !this._layoutRootResizeObserver.IsObserved)
                {
                    if (double.IsNaN(this.Width) || double.IsNaN(this.Height))
                    {
                        _layoutRootResizeObserver.Observe(this.INTERNAL_AdditionalOutsideDivForMargins, (Action<Size>)this.LayoutRootHandleSizeChanged);
                    }
                    else
                    {
                        LayoutRootHandleSizeChanged(new Size(this.Width, this.Height));
                    }
                }
            }
        }

        internal void DetachResizeSensorFromDomElement()
        {
            _resizeObserver.Unobserve(this.INTERNAL_OuterDomElement);

            if (this._layoutRootResizeObserver.IsObserved)
                _layoutRootResizeObserver.Unobserve(this.INTERNAL_AdditionalOutsideDivForMargins);
        }

        public event SizeChangedEventHandler SizeChanged
        {
            add
            {
                if (this._sizeChangedEventHandlers == null)
                {
                    this._sizeChangedEventHandlers = new List<SizeChangedEventHandler>();
                }
                if (!this._resizeObserver.IsObserved && this.INTERNAL_OuterDomElement != null)
                {
                    if (this.IsUnderCustomLayout == false)
                    {
                        if (double.IsNaN(this.Width) || double.IsNaN(this.Height))
                        {
                            _valueOfLastSizeChanged = new Size(0d, 0d);
                            _resizeObserver.Observe(this.INTERNAL_OuterDomElement, (Action<Size>)this.HandleSizeChanged);
                        } 
                        else
                        {
                            HandleSizeChanged(new Size(this.Width, this.Height));
                        }
                    }
                }
                this._sizeChangedEventHandlers.Add(value);
            }
            remove
            {
                if (this._sizeChangedEventHandlers == null)
                {
                    return;
                }

                if (this._sizeChangedEventHandlers.Remove(value))
                {
                    if (this._sizeChangedEventHandlers.Count == 0 && this._resizeObserver.IsObserved)
                    {
                        _resizeObserver.Unobserve(this.INTERNAL_OuterDomElement);
                    }
                }
            }
        }

        internal event SizeChangedEventHandler LayoutRootSizeChanged
        {
            add
            {
                if (this._layoutRootSizeChangedEventHandlers == null)
                {
                    this._layoutRootSizeChangedEventHandlers = new List<SizeChangedEventHandler>();
                }
                if (!this._layoutRootResizeObserver.IsObserved && this.INTERNAL_AdditionalOutsideDivForMargins != null)
                {
                    if (double.IsNaN(this.Width) || double.IsNaN(this.Height))
                    {
                        _valueOfLayoutRootLastSizeChanged = new Size(0d, 0d);
                        _layoutRootResizeObserver.Observe(this.INTERNAL_AdditionalOutsideDivForMargins, (Action<Size>)this.LayoutRootHandleSizeChanged);
                    }
                    else
                    {
                        LayoutRootHandleSizeChanged(new Size(this.Width, this.Height));
                    }
                }
                this._layoutRootSizeChangedEventHandlers.Add(value);
            }
            remove
            {
                if (this._layoutRootSizeChangedEventHandlers == null)
                {
                    return;
                }

                if (this._layoutRootSizeChangedEventHandlers.Remove(value))
                {
                    if (this._layoutRootSizeChangedEventHandlers.Count == 0 && this._layoutRootResizeObserver.IsObserved)
                    {
                        _layoutRootResizeObserver.Unobserve(this.INTERNAL_AdditionalOutsideDivForMargins);
                    }
                }
            }
        }

        #endregion

        #region ContextMenu

        //Note: ContextMenu needs to be at the end of this file because JSIL sometimes causes errors when contructing the Control type (Control inherits from FrameworkElement and ContextMenu inherits Control so we get the error "Recursive construction of type Control")
        //      This causes all the properties that are defined after this one to never be added when constructing the FrameworkElement Type.
        //      cf. project "Chess" or "QSwot".

        /// <summary>
        /// Gets or sets the context menu element that should appear whenever the context menu is requested through user interface (UI) from within this element.
        /// </summary>
        public ContextMenu ContextMenu
        {
            get { return (ContextMenu)GetValue(ContextMenuProperty); }
            set { SetValue(ContextMenuProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.ContextMenu"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register(
                nameof(ContextMenu),
                typeof(ContextMenu),
                typeof(FrameworkElement),
                new PropertyMetadata(null, ContextMenu_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void ContextMenu_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            var contextMenu = (ContextMenu)e.NewValue;

            INTERNAL_ContextMenuHelpers.RegisterContextMenu(frameworkElement, contextMenu);
        }

        /// <summary>
        /// Occurs when any context menu on the element is opened.
        /// </summary>
        public event ContextMenuEventHandler ContextMenuOpening;

        internal void INTERNAL_RaiseContextMenuOpeningEvent(double pointerLeft, double pointerTop)
        {
            if (ContextMenuOpening != null)
                ContextMenuOpening(this, new ContextMenuEventArgs(pointerLeft, pointerTop));
        }

        #endregion
    }
}