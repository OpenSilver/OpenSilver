// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Data;

[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "HierarchicalDataTemplate.get_ItemsSource(), System.Windows.Data.Binding (Silverlight) vs System.Windows.Data.BindingBase (wpf)", Justification = "BindingBase isn't sufficiently rich enough for this purpose.  We use Binding instead, just like FrameworkElement.SetBinding.")]

namespace System.Windows
{
    /// <summary>
    /// Represents a <see cref="DataTemplate" /> that supports
    /// <see cref="HeaderedItemsControl" /> objects,
    /// such as <see cref="TreeViewItem" />.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public partial class HierarchicalDataTemplate : DataTemplate
    {
        /// <summary>
        /// Gets or sets the collection that is used to generate content for the
        /// next sublevel in the data hierarchy.
        /// </summary>
        /// <value>
        /// The collection that is used to generate content for the next
        /// sublevel in the data hierarchy.  The default value is null.
        /// </value>
        public Binding ItemsSource { get; set; }

        #region public DataTemplate ItemTemplate
        /// <summary>
        /// The DataTemplate to apply to the ItemTemplate property on a
        /// generated HeaderedItemsControl (such as a MenuItem or a
        /// TreeViewItem), to indicate how to display items from the next level
        /// in the data hierarchy.
        /// </summary>
        private DataTemplate _itemTemplate;

        /// <summary>
        /// Gets a value indicating whether the ItemTemplate property was set on
        /// the template.
        /// </summary>
        internal bool IsItemTemplateSet { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate" /> to
        /// apply to the
        /// <see cref="ItemsControl.ItemTemplate" />
        /// property on a generated
        /// <see cref="HeaderedItemsControl" />, such
        /// as a <see cref="TreeViewItem" />, to
        /// indicate how to display items from the next sublevel in the data
        /// hierarchy.
        /// </summary>
        /// <value>
        /// The <see cref="DataTemplate" /> to apply to the
        /// <see cref="ItemsControl.ItemTemplate" />
        /// property on a generated
        /// <see cref="HeaderedItemsControl" />, such
        /// as a <see cref="TreeViewItem" />, to
        /// indicate how to display items from the next sublevel in the data
        /// hierarchy.
        /// </value>
        public DataTemplate ItemTemplate
        {
            get { return _itemTemplate; }
            set
            {
                IsItemTemplateSet = true;
                _itemTemplate = value;
            }
        }
        #endregion public DataTemplate ItemTemplate

        #region public Style ItemContainerStyle
        /// <summary>
        /// The Style to apply to the ItemContainerStyle property on a generated
        /// HeaderedItemsControl (such as a MenuItem or a TreeViewItem), to
        /// indicate how to style items from the next level in the data
        /// hierarchy.
        /// </summary>
        private Style _itemContainerStyle;

        /// <summary>
        /// Gets a value indicating whether the ItemContainerStyle property was
        /// set on the template.
        /// </summary>
        internal bool IsItemContainerStyleSet { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Style" /> that is
        /// applied to the item container for each child item.
        /// </summary>
        /// <value>
        /// The style that is applied to the item container for each child item.
        /// </value>
        public Style ItemContainerStyle
        {
            get { return _itemContainerStyle; }
            set
            {
                IsItemContainerStyleSet = true;
                _itemContainerStyle = value;
            }
        }
        #endregion public Style ItemContainerStyle

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HierarchicalDataTemplate" /> class.
        /// </summary>
        public HierarchicalDataTemplate()
        {
        }
    }
}