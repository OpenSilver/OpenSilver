
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
    /// Provides data for the DragDelta event that occurs one or more times when
    /// a user drags a Thumb control with the mouse.
    /// </summary>
    public class DragDeltaEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DragDeltaEventArgs class.
        /// </summary>
        /// <param name="horizontalChange">The horizontal change in the Thumb position since the last DragDelta event.</param>
        /// <param name="verticalChange">The vertical change in the Thumb position since the last DragDelta event.</param>
        public DragDeltaEventArgs(double horizontalChange, double verticalChange)
        {
            this.HorizontalChange = horizontalChange;
            this.VerticalChange = verticalChange;
        }

        /// <summary>
        /// Gets the horizontal change in the Thumb position since the last DragDelta event.
        /// </summary>
        public double HorizontalChange { get; private set; }

        /// <summary>
        /// Gets the vertical change in the Thumb position since the last DragDelta event.
        /// </summary>
        public double VerticalChange { get; private set; }
    }
}
