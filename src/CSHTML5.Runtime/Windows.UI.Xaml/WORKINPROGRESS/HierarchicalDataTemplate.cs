

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
using System.Windows.Controls;
#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    #region Not implemented yet
    public partial class HierarchicalDataTemplate : DataTemplate
    {
        public HierarchicalDataTemplate()
        {
            // Workaround to avoid NullReferenceException since the compiler does not handle HierarchicalDataTemplate yet.
            this._methodToInstantiateFrameworkTemplate = (control) =>
            {
                ContentPresenter presenter = new ContentPresenter();
                presenter.SetBinding(ContentControl.ContentProperty, new Binding());
                return new TemplateInstance()
                {
                    TemplateContent = presenter,
                };
            };
        }

        /// <summary>
        /// Gets or sets the binding that is used to generate content for the next sublevel in the data hierarchy.
        /// </summary>
        public Binding ItemsSource { get; set; }

        /// <summary>
        /// Gets or sets the System.Windows.DataTemplate to apply to the System.Windows.Controls.ItemsControl.ItemTemplate property 
        /// on a generated System.Windows.Controls.HeaderedItemsControl, such as a System.Windows.Controls.TreeViewItem, to indicate
        /// how to display items in the next sublevel in the data hierarchy.
        /// </summary>
        public DataTemplate ItemTemplate { set; get; }
    }
    #endregion
#endif
}
