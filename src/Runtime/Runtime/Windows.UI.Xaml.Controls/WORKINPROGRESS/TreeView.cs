

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
#if WORKINPROGRESS
    [OpenSilver.NotImplemented]
    public partial class TreeView : ItemsControl
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeView), null);

        [OpenSilver.NotImplemented]
        public object SelectedItem
        {
            get { return (object)this.GetValue(TreeView.SelectedItemProperty); }
        }

        /// <summary>
        ///     The DependencyProperty for the <see cref="SelectedValue"/> property.
        ///     Default Value: null
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue",
                typeof(object),
                typeof(TreeView),
                null);

        /// <summary>
        ///     Specifies the a value on the selected item as defined by <see cref="SelectedValuePath" />.
        /// </summary>
        [OpenSilver.NotImplemented]
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

#if MIGRATION
        [OpenSilver.NotImplemented]
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;
#else
#endif
    }
#endif
}
