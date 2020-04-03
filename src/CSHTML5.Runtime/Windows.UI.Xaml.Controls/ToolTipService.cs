

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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.Foundation;
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
    /// Represents a service that provides static methods to display a tooltip.
    /// </summary>
    public static class ToolTipService
    {
        /// <summary>
        /// Identifies the ToolTipService.Placement XAML attached property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached("Placement", typeof(PlacementMode), typeof(ToolTipService), new PropertyMetadata(PlacementMode.Bottom)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the ToolTipService.PlacementTarget XAML attached property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.RegisterAttached("PlacementTarget", typeof(UIElement), typeof(ToolTipService), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Identifies the ToolTipService.ToolTip XAML attached property.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.RegisterAttached("ToolTip", typeof(object), typeof(ToolTipService),
            new PropertyMetadata(null, ToolTip_Changed)
            {
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });

        private static void ToolTip_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DependencyObject element = (DependencyObject)d;
            if (element is UIElement)
            {
                var uiElement = (UIElement)element;
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(uiElement))
                {
                    bool openNewTooltip = false;
                    Point? position = null;
                    ToolTip oldTooltip = uiElement.INTERNAL_AssignedToolTip;
                    if (oldTooltip != null)
                    {
                        openNewTooltip = oldTooltip.IsOpen;
                        position = oldTooltip._forceSpecifyAbsoluteCoordinates;
                        oldTooltip.IsOpen = false;
                    }

                    object newTooltip = (object)e.NewValue;

                    if (newTooltip != null)
                    {
                        // Assign the tooltip:
                        var toolTip = ConvertToToolTip(newTooltip);
                        uiElement.INTERNAL_AssignedToolTip = toolTip;
                        toolTip.INTERNAL_ElementToWhichThisToolTipIsAssigned = uiElement;

                        //open the new ToolTip if the old one was open before changing:
//#if MIGRATION
//                        Point absoluteCoordinates = e.GetPosition(null);
//#else
//                        Point absoluteCoordinates = e.GetCurrentPoint(null).Position;
//#endif
//                        Point absoluteCoordinatesShiftedToBeBelowThePointer = new Point(absoluteCoordinates.X, absoluteCoordinates.Y + 20);
                        if (openNewTooltip)
                        {
                            uiElement.INTERNAL_AssignedToolTip._forceSpecifyAbsoluteCoordinates = position;
                            uiElement.INTERNAL_AssignedToolTip.IsOpen = true;
                        }


                        // Register pointer events: // Note: we unregister before registering in order to ensure that it is only registered once.
#if MIGRATION
                        uiElement.MouseEnter -= UIElement_MouseEnter;
                        uiElement.MouseEnter += UIElement_MouseEnter;
                        uiElement.MouseLeave -= UIElement_MouseLeave;
                        uiElement.MouseLeave += UIElement_MouseLeave;
#else
                        uiElement.PointerEntered -= UIElement_PointerEntered;
                        uiElement.PointerEntered += UIElement_PointerEntered;
                        uiElement.PointerExited -= UIElement_PointerExited;
                        uiElement.PointerExited += UIElement_PointerExited;
#endif
                    }
                    else
                    {
                        // Unregister pointer events:
#if MIGRATION
                        uiElement.MouseEnter -= UIElement_MouseEnter;
                        uiElement.MouseLeave -= UIElement_MouseLeave;
#else
                        uiElement.PointerEntered -= UIElement_PointerEntered;
                        uiElement.PointerExited -= UIElement_PointerExited;
#endif

                        // Unassign tooltip:
                        uiElement.INTERNAL_AssignedToolTip = null;
                    }
                }
            }
        }

        internal static ToolTip ConvertToToolTip(object obj)
        {
            if (obj is ToolTip)
                return (ToolTip)obj;
            else if (obj is UIElement
                && ((UIElement)obj).INTERNAL_VisualParent is ToolTip)
                return (ToolTip)((UIElement)obj).INTERNAL_VisualParent;
            else
                return new ToolTip() { Content = obj };
        }

#if MIGRATION
        static void UIElement_MouseLeave(object sender, MouseEventArgs e)
#else
        static void UIElement_PointerExited(object sender, PointerRoutedEventArgs e)
#endif
        {
            UIElement uielement = (UIElement)sender;

            if (uielement.INTERNAL_AssignedToolTip != null
                && uielement.INTERNAL_AssignedToolTip.IsOpen == true)
                uielement.INTERNAL_AssignedToolTip.IsOpen = false;
        }

#if MIGRATION
        static void UIElement_MouseEnter(object sender, MouseEventArgs e)
#else
        static void UIElement_PointerEntered(object sender, PointerRoutedEventArgs e)
#endif
        {
            UIElement uielement = (UIElement)sender;

            if (uielement.INTERNAL_AssignedToolTip != null
                && uielement.INTERNAL_AssignedToolTip.IsOpen == false)
            {                
#if MIGRATION
                Point absoluteCoordinates = e.GetPosition(null);
#else
                Point absoluteCoordinates = e.GetCurrentPoint(null).Position;
#endif
                Point absoluteCoordinatesShiftedToBeBelowThePointer = new Point(absoluteCoordinates.X, absoluteCoordinates.Y + 20);
                uielement.INTERNAL_AssignedToolTip.INTERNAL_OpenAtCoordinates(absoluteCoordinatesShiftedToBeBelowThePointer);
            }
        }

        /*
        /// <summary>
        /// Gets the ToolTipService.Placement XAML attached property value for the specified target element.
        /// </summary>
        /// <param name="element">The target element for the attached property value.</param>
        /// <returns>The relative position of the specified tooltip.</returns>
        public static PlacementMode GetPlacement(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (PlacementMode)element.GetValue(PlacementProperty);
        }
         */

        /// <summary>Gets the visual element that the tooltip is positioned relative to.</summary>
        /// <returns>The visual element that the tooltip is positioned relative to.</returns>
        /// <param name="element">The tooltip to retrieve the visual element of.</param>
        public static UIElement GetPlacementTarget(DependencyObject element)
        {
            return (UIElement)element.GetValue(ToolTipService.PlacementTargetProperty);
        }

        /// <summary>Sets the position of the specified <see cref="P:System.Windows.Controls.ToolTipService.ToolTip" /> relative to the specified value element.</summary>
        /// <param name="element">The tooltip to set the position of.</param>
        /// <param name="value">The visual element to set the tooltip for.</param>
        public static void SetPlacementTarget(DependencyObject element, UIElement value)
        {
            element.SetValue(ToolTipService.PlacementTargetProperty, (DependencyObject)value);
        }

        /// <summary>
        /// Gets the value of the ToolTipService.ToolTip XAML attached property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's tooltip content.</returns>
        public static object GetToolTip(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (object)element.GetValue(ToolTipProperty);
        }

        /*
        /// <summary>
        /// Sets the ToolTipService.Placement XAML attached property value for the specified target element.
        /// </summary>
        /// <param name="element">The target element for the attached property value.</param>
        /// <param name="value">One of the PlacementMode values, which specifies where the tooltip should appear relative to the control that is the placement target.</param>
        public static void SetPlacement(DependencyObject element, PlacementMode value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementProperty, value);
        }
         */

        /*
        /// <summary>
        /// Sets the ToolTipService.PlacementTarget XAML attached property value for the specified target element.
        /// </summary>
        /// <param name="element">The target element for the attached property value.</param>
        /// <param name="value">The visual element that should be the placement target for the tooltip.</param>
        public static void SetPlacementTarget(DependencyObject element, UIElement value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(PlacementTargetProperty, value);
        }
         */

        /// <summary>
        /// Sets the value of the ToolTipService.ToolTip XAML attached property.
        /// </summary>
        /// <param name="element">The object to set tooltip content on.</param>
        /// <param name="value">The value to set for tooltip content.</param>
        public static void SetToolTip(DependencyObject element, object value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(ToolTipProperty, value);
        }
    }
}
