
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the container for an item in a ListBox control.
    /// </summary>
    public class ComboBoxItem : ListBoxItem
    {
        public ComboBoxItem()
        {
            this.DefaultStyleKey = typeof(ComboBoxItem);
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

            if (parent != null)
            {
                parent.NotifyComboBoxItemMouseUp(this);
            }

            base.OnMouseLeftButtonUp(e);
        }

        internal ComboBox ParentComboBox => ParentSelector as ComboBox;
    }
}
