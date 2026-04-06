
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

using System.Windows.Input;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the container for an item in a ListBox control.
    /// </summary>
    public class ComboBoxItem : ListBoxItem
    {
        private static readonly DependencyPropertyKey IsHighlightedPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsHighlighted),
                typeof(bool),
                typeof(ComboBoxItem),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Identifies the <see cref="IsHighlighted"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsHighlightedProperty = IsHighlightedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether the item is highlighted.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if a <see cref="ComboBoxItem"/> is highlighted; otherwise, <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        [OpenSilver.NotImplemented]
        public bool IsHighlighted
        {
            get => (bool)GetValue(IsHighlightedProperty);
            protected set => SetValue(IsHighlightedPropertyKey, BooleanBoxes.Box(value));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            e.Handled = true;

            ComboBox parent = ParentComboBox;

            parent?.NotifyComboBoxItemMouseUp(this);

            base.OnMouseLeftButtonUp(e);
        }

        internal ComboBox ParentComboBox => ParentSelector as ComboBox;
    }
}
