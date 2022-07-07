// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a CheckBox whose value is associated with the
    /// TreeViewExtensions.IsChecked attached property of TreeViewItems.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateValid, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidFocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = VisualStates.StateInvalidUnfocused, GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = "Unchecked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Indeterminate", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Checked", GroupName = "CheckStates")]
    public sealed partial class TreeViewItemCheckBox : CheckBox
    {
        /// <summary>
        /// The parent TreeViewItem of the CheckBox.
        /// </summary>
        private TreeViewItem _parent;

        /// <summary>
        /// Gets the parent TreeViewItem of the CheckBox.
        /// </summary>
        private TreeViewItem ParentTreeViewItem
        {
            get
            {
                if (_parent == null)
                {
                    AssociateParentTreeViewItem();
                }

                return _parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the TreeViewItemCheckBox class.
        /// </summary>
        public TreeViewItemCheckBox()
        {
            Loaded += (s, e) => AssociateParentTreeViewItem();
            Checked += OnIsCheckedChanged;
            Unchecked += OnIsCheckedChanged;
            Indeterminate += OnIsCheckedChanged;
        }

        /// <summary>
        /// Associate the parent TreeViewItem with the CheckBox.
        /// </summary>
        private void AssociateParentTreeViewItem()
        {
            _parent = TreeViewExtensions.GetParentItemsControl(this) as TreeViewItem;
            if (_parent != null)
            {
                TreeViewExtensions.SetAssociatedCheckBox(_parent, this);
            }
        }

        /// <summary>
        /// Update the TreeViewItem's IsChecked property when this IsChecked
        /// property is changed.
        /// </summary>
        /// <param name="sender">The CheckBox.</param>
        /// <param name="e">Event arguments.</param>
        private void OnIsCheckedChanged(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = ParentTreeViewItem;
            if (item != null)
            {
                TreeViewExtensions.SetIsChecked(item, IsChecked);
            }
        }
    }
}