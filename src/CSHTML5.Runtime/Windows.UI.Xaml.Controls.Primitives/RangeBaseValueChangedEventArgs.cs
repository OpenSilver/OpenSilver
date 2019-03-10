
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
// ----> See the class "System.Windows.RoutedPropertyChangedEventArgs"
#else
namespace Windows.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Provides data about a change in range value for the ValueChanged event.
    /// </summary>
    public sealed class RangeBaseValueChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the RangeBaseValueChangedEventArgs class.
        /// </summary>
        /// <param name="newValue">The new value of a range value property.</param>
        /// <param name="oldValue">The previous value of a range value property.</param>
        public RangeBaseValueChangedEventArgs(double newValue, double oldValue)
        {
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }

        /// <summary>
        /// Gets the new value of a range value property.
        /// </summary>
        public double NewValue { get; private set; }

        /// <summary>
        /// Gets the previous value of a range value property.
        /// </summary>
        public double OldValue { get; private set; }
    }
}
#endif