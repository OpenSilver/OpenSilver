

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
using System.Windows.Threading;
using DotNetForHtml5.Core;

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
        // time to hide tooltip after 5 seconds
        private static DispatcherTimer _timerClose = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 5) };
        private static ToolTip _currentTooltip;
        private static Point? _oldAbsPoistion;
        private static void OnTimerCloseElapsed(object sender, object e)
        {
            CloseToolTip(_currentTooltip);
        }

        /// <summary>
        /// Identifies the ToolTipService.Placement XAML attached property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached("Placement", typeof(PlacementMode), typeof(ToolTipService), new PropertyMetadata(PlacementMode.Bottom));

        /// <summary>
        /// Identifies the ToolTipService.PlacementTarget XAML attached property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.RegisterAttached("PlacementTarget", typeof(UIElement), typeof(ToolTipService), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ToolTipService.ToolTip XAML attached property.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.RegisterAttached("ToolTip", typeof(object), typeof(ToolTipService),
            new PropertyMetadata(null, ToolTip_Changed)
            {
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            });

        /// <summary>
        /// Gets the value of the ToolTipService.ToolTip XAML attached property for an object.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The object's tooltip content.</returns>
        public static ToolTip GetToolTip(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (object)element.GetValue(ToolTipProperty) as ToolTip;
        }

        /// <summary>
        /// Sets the value of the ToolTipService.ToolTip XAML attached property.
        /// </summary>
        /// <param name="element">The object to set tooltip content on.</param>
        /// <param name="value">The value to set for tooltip content.</param>
        public static void SetToolTip(DependencyObject element, object value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (value != null)
            {
                element.SetValue(ToolTipProperty, value);
            }
            else
            {
                element.ClearValue(ToolTipProperty);
            }
        }
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
                    ToolTip oldTooltip = GetToolTip(uiElement);
                    if (oldTooltip != null)
                    {
                        openNewTooltip = oldTooltip.IsOpen;
                        position = _oldAbsPoistion;
                        CloseToolTip(uiElement);
                    }

                    object newTooltip = (object)e.NewValue;

                    if (newTooltip != null)
                    {
                        RegisterToolTipInternal(uiElement, newTooltip);
 
                        if (openNewTooltip)
                        {
                            OpenToolTipAt(uiElement, position);
                        }
                    }
                    else
                    {
                        UnregisterToolTipInternal(uiElement);
                    }
                }
            }
        }

        private static void RegisterToolTipInternal(UIElement uiElement, object newTooltip)
        {
            var toolTip = ConvertToToolTip(newTooltip);
            SetToolTip(uiElement, toolTip);
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

        private static void UnregisterToolTipInternal(UIElement uiElement)
        {
            // Unregister pointer events:
#if MIGRATION
            uiElement.MouseEnter -= UIElement_MouseEnter;
            uiElement.MouseLeave -= UIElement_MouseLeave;
#else
            uiElement.PointerEntered -= UIElement_PointerEntered;
            uiElement.PointerExited -= UIElement_PointerExited;
#endif
            var toolTip = GetToolTip(uiElement);
            SetToolTip(uiElement, null);
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
            UIElement uiElement = (UIElement)sender;
            var toolTip = GetToolTip(uiElement);
            if (toolTip != null && toolTip.IsOpen == true)
            {
                CloseToolTip(uiElement);
            }
        }

#if MIGRATION
        static void UIElement_MouseEnter(object sender, MouseEventArgs e)
#else
        static void UIElement_PointerEntered(object sender, PointerRoutedEventArgs e)
#endif
        {
            UIElement uiElement = (UIElement)sender;
            var toolTip = GetToolTip(uiElement);
            if (toolTip != null && toolTip.IsOpen == false)
            {
#if MIGRATION
                Point absoluteCoordinates = e.GetPosition(null);
#else
                Point absoluteCoordinates = e.GetCurrentPoint(null).Position;
#endif
                Point absoluteCoordinatesShiftedToBeBelowThePointer = new Point(absoluteCoordinates.X, absoluteCoordinates.Y + 20);
                OpenToolTipAt(uiElement, absoluteCoordinatesShiftedToBeBelowThePointer);
            }
        }
        
        private static void ShowTooltip(Point? point, UIElement uiElement = null)
        {
            // Propagate the DataContext:
            _oldAbsPoistion = point;
            if (uiElement != null && uiElement is FrameworkElement)
            {
                _currentTooltip.DataContext = ((FrameworkElement)uiElement).DataContext;
            }

            _currentTooltip.IsOpen = true;

            // Calculate the popup position:
            Popup parentPopup = (Popup)_currentTooltip.Parent;
            if (parentPopup != null)
            {
                // Make sure that the popup is displayed in the same Window as the element to which the ToolTip is assigned. This is useful when there are multiple Windows:
                if (uiElement != null)
                {
                    parentPopup.INTERNAL_ParentWindow = uiElement.INTERNAL_ParentWindow;
                }

                Point? popupAbsolutePosition = null;
                if (point.HasValue)
                {
                    popupAbsolutePosition = point;
                }
                else 
                {
                    if (uiElement != null)
                    {
                        popupAbsolutePosition = INTERNAL_PopupsManager.CalculatePopupAbsolutePositionBasedOnElementPosition(
                                uiElement, _currentTooltip.HorizontalOffset, _currentTooltip.VerticalOffset);
                    }
                }

                // Set the popup position:
                if (popupAbsolutePosition != null)
                {
                    parentPopup.HorizontalOffset = popupAbsolutePosition.Value.X;
                    parentPopup.VerticalOffset = popupAbsolutePosition.Value.Y;
                }
            }

            _timerClose.Tick -= OnTimerCloseElapsed;
            _timerClose.Tick += OnTimerCloseElapsed;
            _timerClose.Start();
        }

        internal static void OpenToolTipAt(UIElement uiElement, Point? point)
        {
            var toolTip = GetToolTip(uiElement);
            if (toolTip != null)
            {
                _currentTooltip = toolTip;
                ShowTooltip(point, uiElement);                
            }
        }


        // This is only used by HtmlCanvasElement
        // TODO: We should handle it better
        internal static void OpenToolTipAt(ToolTip toolTip, Point? point)
        {
            _currentTooltip = toolTip;
            ShowTooltip(point);            
        }

        internal static void CloseToolTip(UIElement uiElement)
        {
            _timerClose.Stop();
            var toolTip = GetToolTip(uiElement);
            if (toolTip != null) toolTip.IsOpen = false;
        }

        // This is only used by HtmlCanvasElement
        // TODO: We should handle it better
        internal static void CloseToolTip(ToolTip toolTip)
        {
            _timerClose.Stop();
            if (toolTip != null) toolTip.IsOpen = false;
        }

  
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
    }
}
