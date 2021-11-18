

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

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the container for an item in a ListBox control.
    /// </summary>
    public partial class ComboBoxItem : ListBoxItem
    {
        public ComboBoxItem()
        {
            this.DefaultStyleKey = typeof(ComboBoxItem);
        }

#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;

            base.OnMouseLeftButtonDown(e);
        }
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            base.OnPointerPressed(e);
        }
#endif

#if MIGRATION
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
#else
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            e.Handled = true;

            ComboBox parent = ParentComboBox;

            if (parent != null)
            {
                parent.NotifyComboBoxItemMouseUp(this);
            }

            base.OnPointerReleased(e);
        }
#endif

        internal ComboBox ParentComboBox
        {
            get 
            { 
                return ParentSelector as ComboBox; 
            }
        }
    }
}
