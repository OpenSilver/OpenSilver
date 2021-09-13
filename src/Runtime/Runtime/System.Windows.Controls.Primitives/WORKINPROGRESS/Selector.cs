

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

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control that allows a user to select an item from a collection of items.
    /// </summary>
    public partial class Selector
    {
        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Primitives.Selector.IsSynchronizedWithCurrentItem" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.Primitives.Selector.IsSynchronizedWithCurrentItem" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty = DependencyProperty.Register(nameof(IsSynchronizedWithCurrentItem), typeof(bool?), typeof(Selector), new PropertyMetadata(OnIsSynchronizedWithCurrentItemChanged));

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="T:System.Windows.Controls.Primitives.Selector" /> should keep the <see cref="P:System.Windows.Controls.Primitives.Selector.SelectedItem" /> synchronized with the current item in the <see cref="P:System.Windows.Controls.ItemsControl.Items" /> property.
        /// </summary>
        /// <returns>
        /// true if the <see cref="P:System.Windows.Controls.Primitives.Selector.SelectedItem" /> is always synchronized with the current item;
        /// false if the <see cref="P:System.Windows.Controls.Primitives.Selector.SelectedItem" /> is never synchronized with the current item;
        /// null if the <see cref="P:System.Windows.Controls.Primitives.Selector.SelectedItem" /> is synchronized with the current item only if the <see cref="T:System.Windows.Controls.Primitives.Selector" /> uses a <see cref="T:System.ComponentModel.ICollectionView" />.
        /// The default is null.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// <see cref="P:System.Windows.Controls.Primitives.Selector.IsSynchronizedWithCurrentItem" /> is set to true.
        /// </exception>
        [OpenSilver.NotImplemented]
        public bool? IsSynchronizedWithCurrentItem
        {
            get => (bool?)GetValue(IsSynchronizedWithCurrentItemProperty);
            set => SetValue(IsSynchronizedWithCurrentItemProperty, value);
        }

        private static void OnIsSynchronizedWithCurrentItemChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
