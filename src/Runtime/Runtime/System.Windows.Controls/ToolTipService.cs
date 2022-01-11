

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
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
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
        private static readonly DispatcherTimer _timerClose;
        private static ToolTip _currentTooltip;

        static ToolTipService()
        {
            _timerClose = new DispatcherTimer
            {
                // time to hide tooltip after 5 seconds
                Interval = new TimeSpan(0, 0, 5)
            };

            _timerClose.Tick += OnTimerCloseElapsed;
        }

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
            new PropertyMetadata(null, ToolTip_Changed));

        private static readonly DependencyProperty ToolTipInternalProperty =
            DependencyProperty.Register(
                "ToolTipInternal",
                typeof(ToolTip),
                typeof(ToolTipService),
                null);

        private static ToolTip GetToolTipInternal(DependencyObject element)
        {
            return (ToolTip)element.GetValue(ToolTipInternalProperty);
        }

        private static void SetToolTipInternal(DependencyObject element, ToolTip tooltip)
        {
            element.SetValue(ToolTipInternalProperty, tooltip);
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

            return element.GetValue(ToolTipProperty);
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

            element.SetValue(ToolTipProperty, value);
        }

        private static void ToolTip_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)d;
            if (e.OldValue != null)
            {
                UnregisterToolTipInternal(uie);
            }
            if (e.NewValue != null)
            {
                RegisterToolTipInternal(uie, e.NewValue);
            }
        }

        private static void RegisterToolTipInternal(UIElement uiElement, object newTooltip)
        {
            ToolTip tooltip = ConvertToToolTip(newTooltip);
            SetToolTipInternal(uiElement, tooltip);
            tooltip.SetOwner(uiElement);
            
#if MIGRATION
            uiElement.MouseEnter += UIElement_MouseEnter;
            uiElement.MouseLeave += UIElement_MouseLeave;
#else
            uiElement.PointerEntered += UIElement_PointerEntered;
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
            ToolTip oldToolTip = GetToolTipInternal(uiElement);
            if (oldToolTip != null && oldToolTip.IsOpen)
            {
                CloseToolTip(oldToolTip);
            }

            SetToolTipInternal(uiElement, null);
        }

        internal static ToolTip ConvertToToolTip(object obj)
        {
            if (obj is ToolTip tooltip)
            {
                return tooltip;
            }

            if (obj is FrameworkElement fe)
            {
                tooltip = fe.Parent as ToolTip;
                if (tooltip != null)
                {
                    return tooltip;
                }
            }

            return new ToolTip { Content = obj };
        }

#if MIGRATION
        private static void UIElement_MouseLeave(object sender, MouseEventArgs e)
#else
        private static void UIElement_PointerExited(object sender, PointerRoutedEventArgs e)
#endif
        {
            UIElement uiElement = (UIElement)sender;
            ToolTip toolTip = GetToolTipInternal(uiElement);
            if (toolTip != null && toolTip.IsOpen == true)
            {
                CloseToolTip(toolTip);
            }
        }

#if MIGRATION
        private static void UIElement_MouseEnter(object sender, MouseEventArgs e)
#else
        private static void UIElement_PointerEntered(object sender, PointerRoutedEventArgs e)
#endif
        {
            UIElement uiElement = (UIElement)sender;
            ToolTip toolTip = GetToolTipInternal(uiElement);
            if (toolTip != null && toolTip.IsOpen == false)
            {
                OpenToolTip(toolTip);
            }
        }

        internal static void OpenToolTip(ToolTip tooltip)
        {
            _currentTooltip = tooltip;
            tooltip.IsOpen = true;
            _timerClose.Start();
        }

        internal static void OpenToolTipAt(ToolTip toolTip, Point? point)
        {
            OpenToolTip(toolTip);
        }

        internal static void CloseToolTip(ToolTip toolTip)
        {
            _timerClose.Stop();

            if (toolTip != null)
                toolTip.IsOpen = false;
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
