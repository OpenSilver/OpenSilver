

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

#if PERFSTAT
            Performance.Counter("Size/Alignment: INTERNAL_InitializeOuterDomElementWidthAndHeight", t0);
#endif
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
                new PropertyMetadata(double.NaN)
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
                new PropertyMetadata(double.NaN)
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
                new PropertyMetadata(HorizontalAlignment.Stretch, HorizontalAlignment_Changed)
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
                    bool isParentAHorizontalStackPanel = fe.INTERNAL_VisualParent is StackPanel && ((StackPanel)fe.INTERNAL_VisualParent).Orientation == Orientation.Horizontal; // If the element is inside a horizontal StackPanel, we ignore the "HorizontalAlignment" property, to ensure that we don't have issues with setting the CSS "display" property of the parent of the "wrapper" element.
                    bool isParentAWrapPanel = fe.INTERNAL_VisualParent is WrapPanel; // If the element is inside a WrapPanel, we ignore the "HorizontalAlignment" property, to ensure that we don't have issues with setting the CSS "display" property of the parent of the "wrapper" element.
                    bool isParentAWrapPanelOrAHorizontalStackPanel = isParentAHorizontalStackPanel || isParentAWrapPanel;
                    var margin = fe.Margin;
                    bool containsNegativeMargins = (margin.Left < 0d || margin.Top < 0d || margin.Right < 0d || margin.Bottom < 0d);

#if !PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS

                    //-----------------------------
                    // Gain access to the styles:
                    //-----------------------------

                    var childOfOuterDomElement = INTERNAL_HtmlDomManager.GetFirstChildDomElement(fe.INTERNAL_OuterDomElement);
                    var styleOfChildOfOuterDomElement = INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(childOfOuterDomElement) ? INTERNAL_HtmlDomManager.GetDomElementStyleForModification(childOfOuterDomElement) : null;
                    var wrapperElement = fe.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny ?? fe.INTERNAL_AdditionalOutsideDivForMargins;
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
                        return;
                    }

                    // If a size in pixels is specified AND the alignment is "Stretch", the behavior is similar to "Center":
                    if (newHorizontalAlignment == HorizontalAlignment.Stretch && !double.IsNaN(fe.Width))
                        newHorizontalAlignment = HorizontalAlignment.Center;

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

                    bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();

                    switch (newHorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            if (!isParentAWrapPanelOrAHorizontalStackPanel)
                            {
                                if (!containsNegativeMargins)
                                {
                                    styleOfWrapperElement.width = "100%";
                                }
                                styleOfOuterDomElement.marginLeft = "0px";
                                styleOfOuterDomElement.marginRight = "auto";
                                if (!(fe is ScrollViewer) && !(fe is WrapPanel)) // Note: we don't know how to handle horizontal alignment properly for the ScrollViewer and the WrapPanel
                                {

                                    if (!isCSSGrid || !(fe is Grid))
                                    {
                                        styleOfOuterDomElement.display = "table";
                                        if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(styleOfChildOfOuterDomElement))
                                        {
                                            //Example of the note below:
                                            //  <Border Width="100" Height="100" Background="#DDDDDD" x:Name="CenterAlignentBorder">
                                            //      <Border HorizontalAlignment="Center" VerticalAlignment="Center" Background="#FFFFAAAA">
                                            //          <TextBlock Text="Center"/>
                                            //      </Border>
                                            //  </Border>
                                            if (styleOfChildOfOuterDomElement.display != "table") //Note: this test was added to prevent a bug that happened when both horizontal and vertical alignment were set, which lead to this line overriding the change of display that happened on a same dom element when the parent did not have a wrapper for its children (I think).
                                            {
                                                // Note: the "if != 'span'" condition below prevents adding "display: table-cell" to elements inside a TextBlock, such as <Run>, <Span>, <Bold>, etc.
                                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
if ($0.tagName.toLowerCase() != 'span')
{
    $0.style.display = 'table-cell';
}
", childOfOuterDomElement);
                                            }
                                        }
                                    }
                                }
                            }
                            styleOfOuterDomElement.width = !double.IsNaN(fe.Width) ? fe.Width.ToInvariantString() + "px" : "auto";
                            break;
                        case HorizontalAlignment.Center:
                            if (!isParentAWrapPanelOrAHorizontalStackPanel)
                            {
                                styleOfWrapperElement.width = "auto"; // Note: in case of an object in a canvas, the "wrapperElement" is the same as the "OuterDomElement", so we need to execute this line before the line that sets the width of the OuterDomElement, otherwise the width is not correctly applied.
                                styleOfOuterDomElement.marginLeft = "auto";
                                styleOfOuterDomElement.marginRight = "auto";
                                if (!(fe is ScrollViewer) && !(fe is WrapPanel)) // Note: we don't know how to handle horizontal alignment properly for the ScrollViewer and the WrapPanel
                                {
                                    if (!isCSSGrid || !(fe is Grid))
                                    {
                                        styleOfOuterDomElement.display = "table";
                                        if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(styleOfChildOfOuterDomElement))
                                        {
                                            //Example of the note below: cf at the same place in case HorizontalAlignment.Left of the switch statement
                                            if (styleOfChildOfOuterDomElement.display != "table") //Note: this test was added to prevent a bug that happened when both horizontal and vertical alignment were set, which lead to this line overriding the change of display that happened on a same dom element when the parent did not have a wrapper for its children (I think).
                                            {
                                                // Note: the "if != 'span'" condition below prevents adding "display: table-cell" to elements inside a TextBlock, such as <Run>, <Span>, <Bold>, etc.
                                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
if ($0.tagName.toLowerCase() != 'span')
{
    $0.style.display = 'table-cell';
}
", childOfOuterDomElement);
                                            }
                                        }
                                    }
                                }
                            }
                            styleOfOuterDomElement.width = !double.IsNaN(fe.Width) ? fe.Width.ToInvariantString() + "px" : "auto";
                            break;
                        case HorizontalAlignment.Right:
                            if (!isParentAWrapPanelOrAHorizontalStackPanel)
                            {
                                styleOfWrapperElement.width = "auto";
                                styleOfOuterDomElement.marginLeft = "auto";
                                styleOfOuterDomElement.marginRight = "0px";
                                if (!(fe is ScrollViewer) && !(fe is WrapPanel)) // Note: we don't know how to handle horizontal alignment properly for the ScrollViewer and the WrapPanel
                                {
                                    if (!isCSSGrid || !(fe is Grid))
                                    {
                                        styleOfOuterDomElement.display = "table";
                                        if (INTERNAL_HtmlDomManager.IsNotUndefinedOrNull(styleOfChildOfOuterDomElement))
                                        {
                                            //Example of the note below: cf at the same place in case HorizontalAlignment.Left of the switch statement
                                            if (styleOfChildOfOuterDomElement.display != "table") //Note: this test was added to prevent a bug that happened when both horizontal and vertical alignment were set, which lead to this line overriding the change of display that happened on a same dom element when the parent did not have a wrapper for its children (I think).
                                            {
                                                // Note: the "if != 'span'" condition below prevents adding "display: table-cell" to elements inside a TextBlock, such as <Run>, <Span>, <Bold>, etc.
                                                CSHTML5.Interop.ExecuteJavaScriptAsync(@"
if ($0.tagName.toLowerCase() != 'span')
{
    $0.style.display = 'table-cell';
}
", childOfOuterDomElement);
                                            }
                                        }
                                    }
                                }
                            }
                            styleOfOuterDomElement.width = !double.IsNaN(fe.Width) ? fe.Width.ToInvariantString() + "px" : "auto";
                            break;
                        case HorizontalAlignment.Stretch:
                            if (!isParentAWrapPanelOrAHorizontalStackPanel)
                            {
                                if (!containsNegativeMargins)
                                {
                                    styleOfWrapperElement.width = "100%";
                                }
                                styleOfOuterDomElement.marginLeft = "0px"; // Default value
                                styleOfOuterDomElement.marginRight = "0px"; // Default value
                                if (fe is StackPanel && ((StackPanel)fe).Orientation == Orientation.Horizontal)
                                {
                                    styleOfOuterDomElement.display = "grid"; // Default value
                                }
                                else
                                {
                                    styleOfOuterDomElement.display = "block"; // Default value
                                }
                                styleOfOuterDomElement.width = "100%"; // Note: We never have both Stretch and a size in pixels, because of the "if" condition at the beginning of this method.
                            }
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
                new PropertyMetadata(VerticalAlignment.Stretch, VerticalAlignment_Changed)
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
                    bool isParentAWrapPanelOrAVerticalStackPanel = fe.INTERNAL_VisualParent is WrapPanel || (fe.INTERNAL_VisualParent is StackPanel && ((StackPanel)fe.INTERNAL_VisualParent).Orientation == Orientation.Vertical); // If the element is inside a WrapPanel or a vertical StackPanel, we ignore the "VerticalAlignment" property, to ensure that we don't have issues with setting the CSS "display" property of the parent of the "wrapper" element.

#if !PREVIOUS_WAY_OF_HANDLING_ALIGNMENTS

                    //-----------------------------
                    // Gain access to the styles:
                    //-----------------------------

                    var wrapperElement = fe.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny ?? fe.INTERNAL_AdditionalOutsideDivForMargins;
                    var styleOfWrapperElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(wrapperElement);
                    var parentOfTheWrapperElement = INTERNAL_HtmlDomManager.GetParentDomElement(wrapperElement);
                    var styleOfParentOfTheWrapperElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(parentOfTheWrapperElement);

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


                    // If a size in pixels is specified AND the alignment is "Stretch", the behavior is similar to "Center":
                    if (newVerticalAlignment == VerticalAlignment.Stretch && !double.IsNaN(fe.Height))
                        newVerticalAlignment = VerticalAlignment.Center;

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

                    bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
                    bool isMsGrid = Grid_InternalHelpers.isMSGrid();
                    switch (newVerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            if (!isParentAWrapPanelOrAVerticalStackPanel)
                            {
                                if (isCSSGrid)
                                {
                                    if ((VisualTreeHelper.GetParent(fe) is Grid))
                                    {
                                        //we get the box sizing element and set the top and bottom margin to auto (see if that could hinder the margins' functionning)
                                        var boxSizingStyle = INTERNAL_HtmlDomManager.GetFrameworkElementBoxSizingStyleForModification(fe);
                                        if (!isMsGrid)
                                        {
                                            boxSizingStyle.marginTop = "0px";
                                            boxSizingStyle.marginBottom = "auto";
                                            //styleOfOuterDomElement.marginTop = "0px";
                                            //styleOfOuterDomElement.marginBottom = "auto";
                                        }
                                        else
                                        {
                                            boxSizingStyle.msGridRowAlign = "start";
                                        }
                                    }
                                }
                                styleOfWrapperElement.verticalAlign = "top";
                            }
                            styleOfOuterDomElement.height = !double.IsNaN(fe.Height) ? fe.Height.ToInvariantString() + "px" : "auto";
                            break;
                        case VerticalAlignment.Center:
                            if (!isParentAWrapPanelOrAVerticalStackPanel)
                            {
                                if (isCSSGrid && (VisualTreeHelper.GetParent(fe) is Grid))
                                {
                                    //we get the box sizing element and set the top and bottom margin to auto (see if that could hinder the margins' functionning)
                                    var boxSizingStyle = INTERNAL_HtmlDomManager.GetFrameworkElementBoxSizingStyleForModification(fe);
                                    if (!isMsGrid)
                                    {
                                        boxSizingStyle.marginTop = "auto";
                                        boxSizingStyle.marginBottom = "auto";

                                        //styleOfOuterDomElement.marginTop = "auto";
                                        //styleOfOuterDomElement.marginBottom = "auto";
                                    }
                                    else
                                    {
                                        boxSizingStyle.msGridRowAlign = "center";
                                    }
                                }
                                else
                                {
                                    styleOfParentOfTheWrapperElement.display = "table";
                                    styleOfWrapperElement.display = "table-cell";
                                }
                                styleOfWrapperElement.verticalAlign = "middle";
                            }
                            styleOfOuterDomElement.height = !double.IsNaN(fe.Height) ? fe.Height.ToInvariantString() + "px" : "auto";
                            break;
                        case VerticalAlignment.Bottom:
                            if (!isParentAWrapPanelOrAVerticalStackPanel)
                            { 
                                if (isCSSGrid && (VisualTreeHelper.GetParent(fe) is Grid))
                                {
                                    //we get the box sizing element and set the top and bottom margin to auto (see if that could hinder the margins' functionning)
                                    var boxSizingStyle = INTERNAL_HtmlDomManager.GetFrameworkElementBoxSizingStyleForModification(fe);
                                    if (!isMsGrid)
                                    {
                                        boxSizingStyle.marginTop = "auto";
                                        boxSizingStyle.marginBottom = "0px";
                                        //styleOfOuterDomElement.marginTop = "auto";
                                        //styleOfOuterDomElement.marginBottom = "0px";

                                    }
                                    else
                                    {
                                        boxSizingStyle.msGridRowAlign = "end";
                                    }
                                }
                                else
                                {
                                    styleOfParentOfTheWrapperElement.display = "table";
                                    styleOfWrapperElement.display = "table-cell";
                                    styleOfWrapperElement.verticalAlign = "bottom";
                                }
                            }
                            styleOfOuterDomElement.height = !double.IsNaN(fe.Height) ? fe.Height.ToInvariantString() + "px" : "auto";
                            break;
                        case VerticalAlignment.Stretch:
                            if (!isParentAWrapPanelOrAVerticalStackPanel)
                            {
                                if (!(isCSSGrid && (VisualTreeHelper.GetParent(fe) is Grid)))
                                {
                                    styleOfWrapperElement.verticalAlign = "middle"; // This is useful when the parent is a horizontal StackPanel
                                    styleOfOuterDomElement.height = "100%";  // Note: We never have both Stretch and a size in pixels, because of the "if" condition at the beginning of this method.
                                }
                                else
                                {
                                    var boxSizingStyle = INTERNAL_HtmlDomManager.GetFrameworkElementBoxSizingStyleForModification(fe);
                                    boxSizingStyle.msGridRowAlign = "stretch";
                                }
                            }
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                //-----------------------------
                // Handle the "Overflow" CSS property:
                //-----------------------------

                /*
                 
                // COMMENTED ON 2016.09.02 because it prevents properly displaying child elements with NEGATIVE MARGINS. To reproduce: put a border with negative margins inside another border.
                 
                if (!(frameworkElement is ScrollViewer) && !(frameworkElement is TextBox)) //Note: The ScrollViewer and the TextBox handle the "overflow" property by itself.
                {
                    // We always display the portions of the child exceeding the edges, unless the element has a fixed size in pixels AND it is not a canvas, or if the element is a TextBox:
                    if (!double.IsNaN(frameworkElement.Height) && !(frameworkElement is Canvas))
                        styleOfOuterDomElement.overflowY = "hidden"; //Note: This value means to crop the portion of the child exceeding the edges.
                    else
                        styleOfOuterDomElement.overflowY = ""; //Note: the default value is "visible"
                }
                 */

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
                new PropertyMetadata(new Thickness())
                {
                    MethodToUpdateDom = Margin_MethodToUpdateDom,
                });

        internal static void Margin_MethodToUpdateDom(DependencyObject d, object newValue)
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif
            var frameworkElement = (FrameworkElement)d;

            Thickness newMargin = (Thickness)newValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(frameworkElement))
            {
                /*
                // Display an error if the user is setting the Margin property AFTER adding the element to the Visual Tree.
                // In fact, due to the optimization made in October 2016, when attaching to the Visual Tree, we create the DIV
                // for margins ONLY if some margins have been set. So it is too late to add the DIV after attached to the visual
                // tree.
                if (oldMargin.Left == 0d && oldMargin.Top == 0d && oldMargin.Bottom == 0d && oldMargin.Right == 0d
                    && (newMargin.Left != 0d || newMargin.Top != 0d || newMargin.Right != 0d || newMargin.Bottom != 0d))
                {
                    if (!_theWarningAboutMarginsHasAlreadyBeenDisplayed && CSHTML5.Interop.IsRunningInTheSimulator)
                    {
                        MessageBox.Show("TIP: For the most accurate result, please set the 'Margin' property BEFORE adding the element to the visual tree.");
                        _theWarningAboutMarginsHasAlreadyBeenDisplayed = true;
                    }
                }
                */
                bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
                bool isMsGrid = Grid_InternalHelpers.isMSGrid();
                // Note: this is used to avoid overwriting the value set for the vertical
                // alignment when in a css Grid, but the msGrid uses another way to set it
                // so we do not need to change what happens here in this case.
                bool isInsideACSSBasedGrid = isCSSGrid && !isMsGrid && frameworkElement.INTERNAL_VisualParent is Grid; 

                var boxSizingElement = frameworkElement.INTERNAL_AdditionalOutsideDivForMargins;
                var styleOfBoxSizingElement = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(boxSizingElement);
                var styleOfOuterDomElement = INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(frameworkElement);

                // todo: if the container has a padding, add it to the margin?
                // Note: positive margins are achieved by setting the "padding" of the outer
                // "box-sizing" element. Negative margins are achieved by setting negative
                // margins to the outer "box-sizing" AND positive "padding" to the element
                // itself (to compensate for the fact that the CSS negative margins will
                // "move" the element instead of making it bigger).

                // ------------- LEFT ---------------
                if (newMargin.Left >= 0)
                {
                    styleOfBoxSizingElement.paddingLeft = newMargin.Left.ToInvariantString() + "px";
                    // This is to "undo" the value that was previously set in case we are
                    // moving from negative margin to positive margin.
                    styleOfBoxSizingElement.marginLeft = ""; 
                }
                else
                {
                    // This is to "undo" the value that was previously set in case we are
                    // moving from positive margin to negative margin.
                    styleOfBoxSizingElement.paddingLeft = ""; 
                    styleOfBoxSizingElement.marginLeft = newMargin.Left.ToInvariantString() + "px";
                }

                // ------------- TOP ---------------
                if (newMargin.Top >= 0)
                {
                    styleOfBoxSizingElement.paddingTop = newMargin.Top.ToInvariantString() + "px";
                    // This is to "undo" the value that was previously set in case we are
                    // moving from negative margin to positive margin.
                    styleOfOuterDomElement.marginTop = ""; 
                }
                else
                {
                    // This is to "undo" the value that was previously set in case we are
                    // moving from positive margin to negative margin.
                    styleOfBoxSizingElement.paddingTop = "";
                    // In case of CSS-based Grid, we cannot mess with the margin proprty
                    // because it is used for vertical alignment (margin "auto").
                    if (!isInsideACSSBasedGrid || 
                        frameworkElement.VerticalAlignment == VerticalAlignment.Top || 
                        frameworkElement.VerticalAlignment == VerticalAlignment.Stretch) 
                    {
                        // Note: vertically we apply negative margins to the "outer dom element"
                        // instead of the "box sizing" element in order to not mess with the CSS
                        // Grid Layout vertical alignment, which uses the margins of the "box sizing"
                        // to apply vertical alignment.
                        styleOfOuterDomElement.marginTop = newMargin.Top.ToInvariantString() + "px"; 
                    }
                }

                // ------------- RIGHT ---------------
                if (newMargin.Right >= 0)
                {
                    styleOfBoxSizingElement.paddingRight = newMargin.Right.ToInvariantString() + "px";
                    // This is to "undo" the value that was previously set in case we are
                    // moving from negative margin to positive margin.
                    styleOfBoxSizingElement.marginRight = ""; 
                }
                else
                {
                    // This is to "undo" the value that was previously set in case we are
                    // moving from positive margin to negative margin.
                    styleOfBoxSizingElement.paddingRight = ""; 
                    styleOfBoxSizingElement.marginRight = newMargin.Right.ToInvariantString() + "px";
                }

                // ------------- BOTTOM ---------------
                if (newMargin.Bottom >= 0)
                {
                    styleOfBoxSizingElement.paddingBottom = newMargin.Bottom.ToInvariantString() + "px";
                    // This is to "undo" the value that was previously set in case we are
                    // moving from negative margin to positive margin.
                    styleOfOuterDomElement.marginBottom = ""; 
                }
                else
                {
                    // This is to "undo" the value that was previously set in case we are
                    // moving from positive margin to negative margin.
                    styleOfBoxSizingElement.paddingBottom = "";
                    // Note: vertically we apply negative margins to the "outer dom element"
                    // instead of the "box sizing" element in order to not mess with the CSS
                    // Grid Layout vertical alignment, which uses the margins of the "box sizing"
                    // to apply vertical alignment.
                    styleOfOuterDomElement.marginBottom = newMargin.Bottom.ToInvariantString() + "px"; 
                }
            }

#if PERFSTAT
            Performance.Counter("Size/Alignment: Margin_Changed", t0);
#endif
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
                new PropertyMetadata(0d, MinHeight_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MinHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(frameworkElement)
                && e.NewValue is double)
            {
                double newValue = (double)e.NewValue;
                var domElementConcernedByTheCssProperty = frameworkElement.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? frameworkElement.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElementConcernedByTheCssProperty);
                if (double.IsNaN(frameworkElement.Height) && style.display == "table")
                    style.height = !double.IsNaN(newValue) && newValue > 0 ? newValue.ToInvariantString() + "px" : "initial";
                else
                    style.minHeight = !double.IsNaN(newValue) && newValue > 0 ? newValue.ToInvariantString() + "px" : "initial";
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
                new PropertyMetadata(0d, MinWidth_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MinWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(frameworkElement)
                && e.NewValue is double)
            {
                double newValue = (double)e.NewValue;
                var domElementConcernedByTheCssProperty = frameworkElement.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? frameworkElement.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElementConcernedByTheCssProperty);
                style.minWidth = !double.IsNaN(newValue) && newValue > 0 ? newValue.ToInvariantString() + "px" : "initial";
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
                new PropertyMetadata(double.PositiveInfinity, MaxHeight_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MaxHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(frameworkElement)
                && e.NewValue is double)
            {
                double newValue = (double)e.NewValue;
                var domElementConcernedByTheCssProperty = frameworkElement.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? frameworkElement.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElementConcernedByTheCssProperty);

                // Commented because at the time of writing "IsInfinity" was not implemented in JSIL:
                //style.maxHeight = !double.IsInfinity(newValue) ? newValue.ToString() + "px" : "initial";
                style.maxHeight = (newValue != double.MaxValue) ? newValue.ToInvariantString() + "px" : "initial";
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
                new PropertyMetadata(double.PositiveInfinity, MaxWidth_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        private static void MaxWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)d;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(frameworkElement)
                && e.NewValue is double)
            {
                double newValue = (double)e.NewValue;
                var domElementConcernedByTheCssProperty = frameworkElement.INTERNAL_OptionalSpecifyDomElementConcernedByMinMaxHeightAndWidth ?? frameworkElement.INTERNAL_OuterDomElement;
                var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domElementConcernedByTheCssProperty);

                // Commented because at the time of writing "IsInfinity" was not implemented in JSIL:
                //style.maxWidth = !double.IsInfinity(newValue) ? newValue.ToString() + "px" : "initial";
                style.maxWidth = (newValue != double.MaxValue) ? newValue.ToInvariantString() + "px" : "initial";
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
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && this.INTERNAL_OuterDomElement != null)
                {
#if !CSHTML5NETSTANDARD
                    if (IsRunningInJavaScript())
                    {
                        return this.INTERNAL_OuterDomElement.offsetWidth;
                    }
                    else
                    {
#endif
                        try
                        {
                            return Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(this.INTERNAL_OuterDomElement, "offsetWidth"));
                        }
                        catch
                        {
                            return 0d;
                        }
#if !CSHTML5NETSTANDARD
                    }
#endif
                }
                else
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
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && this.INTERNAL_OuterDomElement != null)
                {
#if !CSHTML5NETSTANDARD
                    if (IsRunningInJavaScript())
                    {
                        return this.INTERNAL_OuterDomElement.offsetHeight;
                    }
                    else
                    {
#endif
                        try
                        {
                            return Convert.ToDouble(INTERNAL_HtmlDomManager.GetDomElementAttribute(this.INTERNAL_OuterDomElement, "offsetHeight"));
                        }
                        catch
                        {
                            return 0d;
                        }
#if !CSHTML5NETSTANDARD
                    }
#endif
                }
                else
                    return 0d;
            }
        }

        /// <summary>
        /// Use this method for better performance in the Simulator compared to requesting the ActualWidth and ActualHeight separately.
        /// </summary>
        /// <returns>The actual size of the element.</returns>
        internal Size INTERNAL_GetActualWidthAndHeight()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && this.INTERNAL_OuterDomElement != null)
            {
#if !CSHTML5NETSTANDARD
                if (IsRunningInJavaScript())
                {
                    double actualWidth = this.INTERNAL_OuterDomElement.offsetWidth;
                    double actualHeight = this.INTERNAL_OuterDomElement.offsetHeight;
                    return new Size(actualWidth, actualHeight);
                }
                else
                {
#endif
                    try
                    {
                        // Hack to improve the Simulator performance by making only one interop call rather than two:
                        string concatenated = CSHTML5.Interop.ExecuteJavaScript("$0['offsetWidth'].toFixed(3) + '|' + $0['offsetHeight'].toFixed(3)", this.INTERNAL_OuterDomElement).ToString();
                        int sepIndex = concatenated != null ? concatenated.IndexOf('|') : -1;
                        if (sepIndex > -1)
                        {
                            string actualWidthAsString = concatenated.Substring(0, sepIndex);
                            string actualHeightAsString = concatenated.Substring(sepIndex + 1);
                            double actualWidth = double.Parse(actualWidthAsString, global::System.Globalization.CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                            double actualHeight = double.Parse(actualHeightAsString, global::System.Globalization.CultureInfo.InvariantCulture); //todo: read note above
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
#if !CSHTML5NETSTANDARD
                }
#endif
            }
            else
                return new Size(0d, 0d);
        }

        /// <summary>
        /// Use this method for better performance in the Simulator compared to requesting the ActualWidth and ActualHeight separately.
        /// </summary>
        /// <returns>The actual size of the element.</returns>
        internal Size INTERNAL_GetActualWidthAndHeightUsinggetboudingClientRect()
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this) && this.INTERNAL_OuterDomElement != null)
            {
#if !CSHTML5NETSTANDARD
                if (IsRunningInJavaScript())
                {
                    var rect = this.INTERNAL_OuterDomElement.getBoundingClientRect();
                    double actualWidth = rect.width;
                    double actualHeight = rect.height;
                    return new Size(actualWidth, actualHeight);
                }
                else
                {
#endif
                    try
                    {
                        // Hack to improve the Simulator performance by making only one interop call rather than two:
                        string concatenated = CSHTML5.Interop.ExecuteJavaScript("(function() { var v = $0.getBoundingClientRect(); return v.width.toFixed(3) + '|' + v.height.toFixed(3) })()", this.INTERNAL_OuterDomElement).ToString();
                        int sepIndex = concatenated != null ? concatenated.IndexOf('|') : -1;
                        if (sepIndex > -1)
                        {
                            string actualWidthAsString = concatenated.Substring(0, sepIndex);
                            string actualHeightAsString = concatenated.Substring(sepIndex + 1);
                            double actualWidth = double.Parse(actualWidthAsString, global::System.Globalization.CultureInfo.InvariantCulture); //todo: verify that the locale is OK. I think that JS by default always produces numbers in invariant culture (with "." separator).
                            double actualHeight = double.Parse(actualHeightAsString, global::System.Globalization.CultureInfo.InvariantCulture); //todo: read note above
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
#if !CSHTML5NETSTANDARD
                }
#endif
            }
            else
                return new Size(0d, 0d);
        }

        #endregion


        #region SizeChanged

        Size _valueOfLastSizeChanged = new Size(0d, 0d);
        private List<SizeChangedEventHandler> _sizeChangedEventHandlers;
        private object _resizeSensor;

        private void HandleSizeChanged()
        {
            if (this._sizeChangedEventHandlers != null
               && this._sizeChangedEventHandlers.Count > 0
               && INTERNAL_VisualTreeManager.IsElementInVisualTree(this)
               && this._isLoaded)
            {
                // In the current implementation, we raise the SizeChanged event only if the size has changed since the last time that we were supposed to raise the event:

                Size currentSize = this.INTERNAL_GetActualWidthAndHeight();

                if (!Size.Equals(this._valueOfLastSizeChanged, currentSize))
                {
                    this._valueOfLastSizeChanged = currentSize;

                    // Raise the "SizeChanged" event of all the listeners:
                    foreach (var sizeChangedEventHandler in this._sizeChangedEventHandlers)
                    {
                        sizeChangedEventHandler(this, new SizeChangedEventArgs(currentSize));
                    }
                }
            }
        }

        internal void INTERNAL_SizeChangedWhenAttachedToVisualTree() // Intended to be called by the "VisualTreeManager" when the FrameworkElement is attached to the visual tree.
        {
            // We reset the previous size value so that the SizeChanged event can be called (see the comment in "HandleSizeChanged"):
            _valueOfLastSizeChanged = Size.Empty;
            HandleSizeChanged();

            if (this._sizeChangedEventHandlers != null &&
                this._sizeChangedEventHandlers.Count > 0 &&
                this._resizeSensor == null)
            {
                object sensor = CSHTML5.Interop.ExecuteJavaScript(@"new ResizeSensor($0, $1)", this.INTERNAL_OuterDomElement, (Action)this.HandleSizeChanged);
                this._resizeSensor = sensor;
            }
        }

        public event SizeChangedEventHandler SizeChanged
        {
            add
            {
                if (this._sizeChangedEventHandlers == null)
                {
                    this._sizeChangedEventHandlers = new List<SizeChangedEventHandler>();
                }
                if (this._resizeSensor == null && this.INTERNAL_OuterDomElement != null)
                {
                    object sensor = CSHTML5.Interop.ExecuteJavaScript(@"new ResizeSensor($0, $1)", this.INTERNAL_OuterDomElement, (Action)this.HandleSizeChanged);
                    this._resizeSensor = sensor;
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
                    if (this._sizeChangedEventHandlers.Count == 0 && this._resizeSensor != null)
                    {
                        CSHTML5.Interop.ExecuteJavaScript("$0.detach($1)", this._resizeSensor, this.INTERNAL_OuterDomElement);
                        this._resizeSensor = null;
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