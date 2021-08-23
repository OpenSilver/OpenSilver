// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Collapse()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.Expand()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.IExpandCollapseProvider.ExpandCollapseState", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.IGridProvider.ColumnCount", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.IGridProvider.GetItem(System.Int32,System.Int32)", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.IGridProvider.RowCount", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.IScrollItemProvider.ScrollIntoView()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.CanSelectMultiple", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.GetSelection()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridGroupItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.IsSelectionRequired", Justification = "Base functionality is available through the GetPattern method.")]

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// AutomationPeer for a group of items in a DataGrid
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataGridGroupItemAutomationPeer : FrameworkElementAutomationPeer,
        IExpandCollapseProvider, IGridProvider, IScrollItemProvider, ISelectionProvider
    {
#region Data

        private CollectionViewGroup _group;
        private AutomationPeer _dataGridAutomationPeer;

#endregion

#region Constructors

        /// <summary>
        /// AutomationPeer for a group of items in a DataGrid
        /// </summary>
        public DataGridGroupItemAutomationPeer(CollectionViewGroup group, DataGrid dataGrid)
            : base(dataGrid)
        {
            if (group == null)
            {
                // 



                throw new ElementNotAvailableException();
            }
            if (dataGrid == null)
            {
                // 



                throw new ElementNotAvailableException();
            }

            _group = group;
            _dataGridAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(dataGrid);
        }

#endregion

#region Properties

        /// <summary>
        /// The owning DataGrid
        /// </summary>
        private DataGrid OwningDataGrid
        {
            get
            {
                DataGridAutomationPeer gridPeer = _dataGridAutomationPeer as DataGridAutomationPeer;
                return (DataGrid)gridPeer.Owner;
            }
        }

        /// <summary>
        /// The owning DataGrid's Automation Peer
        /// </summary>
        private DataGridAutomationPeer OwningDataGridPeer
        {
            get
            {
                return _dataGridAutomationPeer as DataGridAutomationPeer;
            }
        }

        /// <summary>
        /// The owning DataGridRowGroupHeader
        /// </summary>
        private DataGridRowGroupHeader OwningRowGroupHeader
        {
            get
            {
                if (this.OwningDataGrid != null)
                {
                    DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                    if (groupInfo != null && this.OwningDataGrid.IsSlotVisible(groupInfo.Slot))
                    {
                        return this.OwningDataGrid.DisplayData.GetDisplayedElement(groupInfo.Slot) as DataGridRowGroupHeader;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// The owning DataGridRowGroupHeader's Automation Peer
        /// </summary>
        internal DataGridRowGroupHeaderAutomationPeer OwningRowGroupHeaderPeer
        {
            get
            {
                DataGridRowGroupHeaderAutomationPeer rowGroupHeaderPeer = null;
                DataGridRowGroupHeader rowGroupHeader = this.OwningRowGroupHeader;
                if (rowGroupHeader != null)
                {
                    rowGroupHeaderPeer = FrameworkElementAutomationPeer.FromElement(rowGroupHeader) as DataGridRowGroupHeaderAutomationPeer;
                    if (rowGroupHeaderPeer == null)
                    {
                        rowGroupHeaderPeer = FrameworkElementAutomationPeer.CreatePeerForElement(rowGroupHeader) as DataGridRowGroupHeaderAutomationPeer;
                    }
                }
                return rowGroupHeaderPeer;
            }
        }

#endregion

#region AutomationPeer Overrides

        ///
        protected override string GetAcceleratorKeyCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetAcceleratorKey() : string.Empty;
        }

        ///
        protected override string GetAccessKeyCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetAccessKey() : string.Empty;
        }

        ///
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        ///
        protected override string GetAutomationIdCore()
        {
            // The AutomationId should be unset for dynamic content.
            return string.Empty;
        }

        ///
        protected override Rect GetBoundingRectangleCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetBoundingRectangle() : new Rect();
        }

        ///
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required method signature for automation peers")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> children = new List<AutomationPeer>();
            if (this.OwningRowGroupHeaderPeer != null)
            {
                this.OwningRowGroupHeaderPeer.InvalidatePeer();
                children.AddRange(this.OwningRowGroupHeaderPeer.GetChildren());
            }
            if (_group.IsBottomLevel)
            {
                foreach (object item in _group.Items)
                {
                    children.Add(this.OwningDataGridPeer.GetOrCreateItemPeer(item));
                }
            }
            else
            {
                foreach (object group in _group.Items)
                {
                    children.Add(this.OwningDataGridPeer.GetOrCreateGroupItemPeer(group));
                }
            }
            return children;
        }

        ///
        protected override string GetClassNameCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetClassName() : string.Empty;
        }

        ///
        protected override Point GetClickablePointCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetClickablePoint() : new Point(double.NaN, double.NaN);
        }

        ///
        protected override string GetHelpTextCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetHelpText() : string.Empty;
        }

        ///
        protected override string GetItemStatusCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetItemStatus() : string.Empty;
        }

        ///
        override protected string GetItemTypeCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetItemType() : string.Empty;
        }

        ///
        protected override AutomationPeer GetLabeledByCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetLabeledBy() : null;
        }

        ///
        protected override string GetLocalizedControlTypeCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetLocalizedControlType() : string.Empty;
        }

        ///
        protected override string GetNameCore()
        {
            if (_group.Name != null)
            {
                string name = _group.Name.ToString();
                if (name != null)
                {
                    return name;
                }
            }
            return base.GetNameCore();
        }

        ///
        protected override AutomationOrientation GetOrientationCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.GetOrientation() : AutomationOrientation.None;
        }

        ///
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.ExpandCollapse:
                case PatternInterface.Grid:
                case PatternInterface.Selection:
                case PatternInterface.Table:
                    return this;
                case PatternInterface.ScrollItem:
                    {
                        if (this.OwningDataGrid.VerticalScrollBar != null &&
                            this.OwningDataGrid.VerticalScrollBar.Maximum > 0)
                        {
                            return this;
                        }
                        break;
                    }
            }
            return null;
        }

        ///
        protected override bool HasKeyboardFocusCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.HasKeyboardFocus() : false;
        }

        ///
        protected override bool IsContentElementCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.IsContentElement() : true;
        }

        ///
        protected override bool IsControlElementCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.IsControlElement() : true;
        }

        ///
        protected override bool IsEnabledCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.IsEnabled() : false;
        }

        ///
        protected override bool IsKeyboardFocusableCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.IsKeyboardFocusable() : false;
        }

        ///
        protected override bool IsOffscreenCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.IsOffscreen() : true;
        }

        ///
        protected override bool IsPasswordCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.IsPassword() : false;
        }

        ///
        protected override bool IsRequiredForFormCore()
        {
            return (this.OwningRowGroupHeaderPeer != null) ? this.OwningRowGroupHeaderPeer.IsRequiredForForm() : false;
        }

        ///
        protected override void SetFocusCore()
        {
            if (this.OwningRowGroupHeaderPeer != null)
            {
                this.OwningRowGroupHeaderPeer.SetFocus();
            }
        }

#endregion

#region IExpandCollapseProvider Members

        void IExpandCollapseProvider.Collapse()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null)
            {
                this.OwningDataGrid.CollapseRowGroup(_group, false /*collapseAllSubgroups*/);
            }
        }

        void IExpandCollapseProvider.Expand()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null)
            {
                this.OwningDataGrid.ExpandRowGroup(_group, false /*expandAllSubgroups*/);
            }
        }

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                if (this.OwningDataGrid != null)
                {
                    DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                    if (groupInfo != null && groupInfo.Visibility == Visibility.Visible)
                    {
                        return ExpandCollapseState.Expanded;
                    }
                }
                return ExpandCollapseState.Collapsed;
            }
        }

#endregion

#region IGridProvider Members

        int IGridProvider.ColumnCount
        {
            get
            {
                if (this.OwningDataGrid != null)
                {
                    return this.OwningDataGrid.Columns.Count;
                }
                return 0;
            }
        }

        IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null &&
                this.OwningDataGrid.DataConnection != null &&
                row >= 0 && row < _group.ItemCount &&
                column >= 0 && column < this.OwningDataGrid.Columns.Count)
            {
                DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                if (groupInfo != null)
                {
                    // Adjust the row index to be relative to the DataGrid instead of the group
                    row = groupInfo.Slot - this.OwningDataGrid.RowGroupHeadersTable.GetIndexCount(0, groupInfo.Slot) + row + 1;
                    Debug.Assert(row >= 0 && row < this.OwningDataGrid.DataConnection.Count);
                    int slot = this.OwningDataGrid.SlotFromRowIndex(row);

                    if (!this.OwningDataGrid.IsSlotVisible(slot))
                    {
                        object item = this.OwningDataGrid.DataConnection.GetDataItem(row);
                        this.OwningDataGrid.ScrollIntoView(item, this.OwningDataGrid.Columns[column]);
                    }
                    Debug.Assert(this.OwningDataGrid.IsSlotVisible(slot));

                    DataGridRow dgr = this.OwningDataGrid.DisplayData.GetDisplayedElement(slot) as DataGridRow;

                    // the first cell is always the indentation filler cell if grouping is enabled, so skip it
                    Debug.Assert(column + 1 < dgr.Cells.Count);
                    DataGridCell cell = dgr.Cells[column + 1];
                    AutomationPeer peer = CreatePeerForElement(cell);
                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }
            }
            return null;
        }

        int IGridProvider.RowCount
        {
            get
            {
                return _group.ItemCount;
            }
        }

#endregion

#region IScrollItemProvider

        void IScrollItemProvider.ScrollIntoView()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null)
            {
                DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                if (groupInfo != null)
                {
                    this.OwningDataGrid.ScrollIntoView(groupInfo.CollectionViewGroup, null);
                }
            }
        }

#endregion

#region ISelectionProvider

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            EnsureEnabled();

            if (this.OwningDataGrid != null &&
                this.OwningDataGridPeer != null &&
                this.OwningDataGrid.SelectedItems != null &&
                _group.ItemCount > 0)
            {
                DataGridRowGroupInfo groupInfo = this.OwningDataGrid.RowGroupInfoFromCollectionViewGroup(_group);
                if (groupInfo != null)
                {
                    // See which of the selected items are contained within this group
                    List<IRawElementProviderSimple> selectedProviders = new List<IRawElementProviderSimple>();
                    int startRowIndex = groupInfo.Slot - this.OwningDataGrid.RowGroupHeadersTable.GetIndexCount(0, groupInfo.Slot) + 1;
                    foreach (object item in this.OwningDataGrid.GetSelectionInclusive(startRowIndex, startRowIndex + _group.ItemCount - 1))
                    {
                        DataGridItemAutomationPeer peer = this.OwningDataGridPeer.GetOrCreateItemPeer(item);
                        if (peer != null)
                        {
                            selectedProviders.Add(ProviderFromPeer(peer));
                        }
                    }
                    return selectedProviders.ToArray();
                }
            }
            return null;
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                return (this.OwningDataGrid != null && this.OwningDataGrid.SelectionMode == DataGridSelectionMode.Extended);
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

#endregion

#region Private Methods

        private void EnsureEnabled()
        {
            if (!_dataGridAutomationPeer.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

#endregion
    }
}
