
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
    /// Provides data for the Scroll event.
    /// </summary>
    public sealed class ScrollEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ScrollEventArgs class.
        /// </summary>
        /// <param name="newValue">The new Value of the ScrollBar.</param>
        /// <param name="scrollEventType">A ScrollEventType describing the event.</param>
        public ScrollEventArgs(double newValue, ScrollEventType scrollEventType)
        {
            this.NewValue = newValue;
            this.ScrollEventType = scrollEventType;
        }

        /// <summary>
        /// Gets the new Value of the ScrollBar.
        /// </summary>
        public double NewValue { get; private set; }

        /// <summary>
        /// Gets a ScrollEventType describing the event.
        /// </summary>
        public ScrollEventType ScrollEventType { get; private set; }
    }
}
