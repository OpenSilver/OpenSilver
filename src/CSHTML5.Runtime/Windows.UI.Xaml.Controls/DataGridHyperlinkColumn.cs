
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a System.Windows.Controls.DataGrid column that hosts System.Uri
    /// elements in its cells.
    /// </summary>
    public class DataGridHyperlinkColumn : DataGridBoundColumn
    {
        /// <summary>
        /// Gets or sets the binding to the text of the hyperlink.
        /// </summary>
        public BindingBase ContentBinding
        {
            get { return (BindingBase)GetValue(ContentBindingProperty); }
            set { SetValue(ContentBindingProperty, value); }
        }
        /// <summary>
        /// Identifies the System.Windows.Controls.DataGridBoundColumn.Binding dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentBindingProperty =
            DependencyProperty.Register("ContentBinding", typeof(BindingBase), typeof(DataGridBoundColumn), new PropertyMetadata(null, ContentBinding_Changed));


        static void ContentBinding_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: tell the DataGrid that the Binding has changed so it has to refresh the elements (only i it is already in the visual tree).
        }


        internal override FrameworkElement GenerateEditingElement(object childData)
        {

            //we should probably simply make a TextBox
            TextBox textBox = new TextBox();
            textBox.DataContext = childData;
            Binding b = this.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty); //we get the Binding in the Binding property set by the user.
            if (b != null)
            {
                if (b.Mode == BindingMode.OneWay)
                {
                    if (!b.INTERNAL_WasModeSetByUserRatherThanDefaultValue())
                    {
                        b.Mode = BindingMode.TwoWay;
                    }
                }
                textBox.SetBinding(TextBox.TextProperty, b);
            }
            else if (childData is string)
                textBox.Text = (string)childData;
            return textBox;
        }

        internal override FrameworkElement GenerateElement(object childData)
        {
            HyperlinkButton hyperlink = new HyperlinkButton();
            hyperlink.DataContext = childData;
            Binding contentBinding = this.INTERNAL_GetBinding(DataGridHyperlinkColumn.ContentBindingProperty); //we get the Binding in the ContentBinding property set by the user.
            Binding b = this.INTERNAL_GetBinding(DataGridBoundColumn.BindingProperty); //we get the Binding in the Binding property set by the user.
            if (b != null)
            {
                if (b.Mode == BindingMode.OneWay)
                {
                    if (!b.INTERNAL_WasModeSetByUserRatherThanDefaultValue())
                    {
                        b.Mode = BindingMode.TwoWay;
                    }
                }
                hyperlink.SetBinding(HyperlinkButton.NavigateUriProperty, b);
            }
            if (contentBinding != null)
            {
                if (contentBinding.Mode == BindingMode.OneWay)
                {
                    if (!contentBinding.INTERNAL_WasModeSetByUserRatherThanDefaultValue())
                    {
                        contentBinding.Mode = BindingMode.TwoWay;
                    }
                }
                hyperlink.SetBinding(HyperlinkButton.ContentProperty, contentBinding);
            }
            else if (b != null)
            {
                hyperlink.SetBinding(HyperlinkButton.ContentProperty, b);
            }
            else if (childData is string)
            {
                hyperlink.NavigateUri = new Uri((string)childData);
                hyperlink.Content = (string)childData;
            }
            return hyperlink;
        }
    }
}
