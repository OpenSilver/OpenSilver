

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
using CSHTML5.Native.Html.Controls;
using System.Threading.Tasks;
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a pop-up menu that enables a control to expose functionality that is specific to the context of the control.
    /// </summary>
    public partial class ContextMenu : MenuBase
    {
        Popup _parentPopup;
        Point? _forceSpecifyAbsoluteCoordinates;
        bool _isClickingInsidePopup; // Note: this is used to distinguish between a click inside the menu and a click inside the surrounding Canvas.
        internal UIElement INTERNAL_ElementToWhichThisMenuIsAssigned;
        internal HtmlCanvasElement INTERNAL_HtmlCanvasElementToWhichThisMenuIsAssigned;

        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.ContextMenu class.
        /// </summary>
        public ContextMenu()
        {
        }


        public void INTERNAL_OpenAtCoordinates(Point absoluteCoordinates)
        {
            _forceSpecifyAbsoluteCoordinates = absoluteCoordinates;

            this.IsOpen = true;
        }


        //-----------------------
        // ISOPEN
        //-----------------------

        /// <summary>
        /// Gets or sets a value that indicates whether the ContextMenu is visible. True if the ContextMenu is visible; otherwise, false. The default is false.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the IsOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ContextMenu), new PropertyMetadata(false, IsOpen_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        private static void IsOpen_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextMenu contextMenu = (ContextMenu)d;
            if (e.NewValue is bool)
            {
                bool isOpen = (bool)e.NewValue;
                if (isOpen)
                {
                    bool wasPopupAlreadyOpen = (contextMenu._parentPopup != null && contextMenu._parentPopup.IsOpen == true);
                    if (!wasPopupAlreadyOpen) // Note: this prevents loops due to the fact that when the popup opens, the "ToolTip.IsOpen_Changed" method is called because the tooltip is the child of the Popup so its properties are called when it is loaded into the Visual Tree.
                    {
                        // Reset the "_isClickingInsidePopup" field (see note near the definition of the field):
                        contextMenu._isClickingInsidePopup = false;

                        // Propagate the DataContext:
                        if (contextMenu.INTERNAL_ElementToWhichThisMenuIsAssigned is FrameworkElement)
                            contextMenu.DataContext = ((FrameworkElement)contextMenu.INTERNAL_ElementToWhichThisMenuIsAssigned).DataContext;

                        // Calculate the menu position:
                        Point menuAbsolutePosition;
                        if (contextMenu._forceSpecifyAbsoluteCoordinates.HasValue)
                        {
                            menuAbsolutePosition = contextMenu._forceSpecifyAbsoluteCoordinates.Value;
                        }
                        else
                        {
                            menuAbsolutePosition = INTERNAL_PopupsManager.CalculatePopupAbsolutePositionBasedOnElementPosition(
                                    contextMenu.INTERNAL_ElementToWhichThisMenuIsAssigned,
                                    contextMenu.HorizontalOffset,
                                    contextMenu.VerticalOffset);
                        }

                        // Make sure the menu is Top/Left-aligned inside the full-screen popup:
                        contextMenu.HorizontalAlignment = HorizontalAlignment.Left;
                        contextMenu.VerticalAlignment = VerticalAlignment.Top;
                        Canvas.SetLeft(contextMenu, menuAbsolutePosition.X);
                        Canvas.SetTop(contextMenu, menuAbsolutePosition.Y);

                        // Create the popup if not already created:
                        if (contextMenu._parentPopup == null)
                        {
                            // Place the menu inside a Canvas so that we can position it with Left/Top:
                            var canvas = new Canvas()
                                {
                                    Background = new SolidColorBrush(Colors.Transparent),
                                    HorizontalAlignment = HorizontalAlignment.Stretch,
                                    VerticalAlignment = VerticalAlignment.Stretch
                                };
                            canvas.Children.Add(contextMenu);

                            // Create the popup:
                            contextMenu._parentPopup = new Popup()
                            {
                                Child = canvas,

                                // We want the popup to be full-screen (to catch the clicks outside the menu):
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                VerticalContentAlignment = VerticalAlignment.Stretch
                            };

                            // Clicking on the canvas will close the popup:
#if MIGRATION
                            canvas.MouseLeftButtonUp += (s2, e2) =>
#else
                            canvas.PointerReleased += (s2, e2) =>
#endif
                                {
                                    if (!contextMenu._isClickingInsidePopup) // We verify that the click does not originate from withing the menu, because we only want to close the popup if the user has clicked outside the menu.
                                        contextMenu.IsOpen = false;
                                };

                            // ...Unless the user has clicked inside the menu:
#if MIGRATION
                            contextMenu.MouseLeftButtonDown += (s2, e2) =>
#else
                            contextMenu.PointerPressed += (s2, e2) =>
#endif
                            {
                                contextMenu._isClickingInsidePopup = true;
                            };
#if MIGRATION
                            contextMenu.MouseLeftButtonUp += (s2, e2) =>
#else
                            contextMenu.PointerReleased += (s2, e2) =>
#endif
                            {
                                // We execute on the Dispatcher queue in order to ensure that the code happens AFTER the PointerReleased of the Canvas (see above):
                                contextMenu.Dispatcher.BeginInvoke(async () =>
                                    {
                                        await Task.Delay(200);
                                        contextMenu._isClickingInsidePopup = false;
                                        contextMenu.IsOpen = false;
                                    });
                            };
                        }

                        // Open the popup:
                        contextMenu._parentPopup.IsOpen = true;

                        // Raise the "Opened" event:
                        contextMenu.OnOpened(new RoutedEventArgs());
                    }
                }
                else
                {
                    if (contextMenu._parentPopup != null
                        && contextMenu._parentPopup.IsOpen == true)
                    {
                        contextMenu._parentPopup.IsOpen = false;

                        // Raise the "Closed" event:
                        contextMenu.OnClosed(new RoutedEventArgs());
                    }
                }
            }
        }


        //-----------------------
        // HORIZONTALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the horizontal distance between the target origin and the pop-up alignment point. The default is 0.
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the HorizontalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ContextMenu), new PropertyMetadata(0d)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        //-----------------------
        // VERTICALOFFSET
        //-----------------------

        /// <summary>
        /// Gets or sets the vertical distance between the target origin and the pop-up alignment point. The default is 0.
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the VerticalOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ContextMenu), new PropertyMetadata(0d)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        #region Opened/Closed events-related members

        /// <summary>
        /// Occurs when a particular instance of a System.Windows.Controls.ContextMenu closes.
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Occurs when a particular instance of a context menu opens.
        /// </summary>
        public event RoutedEventHandler Opened;

        /// <summary>
        /// Called when the System.Windows.Controls.ContextMenu.Closed event occurs.
        /// </summary>
        /// <param name="e">The event data for the System.Windows.Controls.ContextMenu.Closed event.</param>
        protected virtual void OnClosed(RoutedEventArgs e)
        {
            if (this.Closed != null)
                this.Closed(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Called when the System.Windows.Controls.ContextMenu.Opened event occurs.
        /// </summary>
        /// <param name="e">The event data for the System.Windows.Controls.ContextMenu.Opened event.</param>
        protected virtual void OnOpened(RoutedEventArgs e)
        {
            if (this.Opened != null)
                this.Opened(this, new RoutedEventArgs());
        }

        #endregion
    }
}
