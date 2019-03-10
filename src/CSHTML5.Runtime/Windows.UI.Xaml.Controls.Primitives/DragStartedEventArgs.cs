
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Provides data for the DragStarted event that occurs when a user drags a Thumb control with the mouse.
    /// </summary>
    public class DragStartedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DragStartedEventArgs class.
        /// </summary>
        /// <param name="horizontalOffset">The horizontal distance between the current mouse position and the thumb coordinates.</param>
        /// <param name="verticalOffset">The vertical distance between the current mouse position and the thumb coordinates.</param>
        public DragStartedEventArgs(double horizontalOffset, double verticalOffset)
        {
            this.HorizontalOffset = horizontalOffset;
            this.VerticalOffset = verticalOffset;
        }

        /// <summary>
        /// Gets the horizontal distance between the current mouse position and the thumb
        /// coordinates.
        /// </summary>
        public double HorizontalOffset { get; private set; }

        /// <summary>
        /// Gets the vertical distance between the current mouse position and the thumb
        /// coordinates.
        /// </summary>
        public double VerticalOffset { get; private set; }
    }
}
