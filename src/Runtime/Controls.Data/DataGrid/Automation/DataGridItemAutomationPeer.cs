// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.IInvokeProvider.Invoke()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.IScrollItemProvider.ScrollIntoView()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.AddToSelection()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.IsSelected", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.RemoveFromSelection()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.Select()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.SelectionContainer", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.CanSelectMultiple", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.GetSelection()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridItemAutomationPeer.#System.Windows.Automation.Provider.ISelectionProvider.IsSelectionRequired", Justification = "Base functionality is available through the GetPattern method.")]

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// AutomationPeer for an item in a DataGrid
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridItemAutomationPeer : FrameworkElementAutomationPeer,
        IInvokeProvider, IScrollItemProvider, ISelectionItemProvider, ISelectionProvider
    {
#region Data

        private object _item;
        private AutomationPeer _dataGridAutomationPeer;

#endregion

#region Constructors

        /// <summary>
        /// AutomationPeer for an item in a DataGrid
        /// </summary>
        public DataGridItemAutomationPeer(object item, DataGrid dataGrid)
            : base(dataGrid)
        {
            if (item == null)
            {
                // 



                throw new ElementNotAvailableException();
            }
            if (dataGrid == null)
            {
                // 



                throw new ElementNotAvailableException();
            }

            _item = item;
            _dataGridAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(dataGrid);
        }

#endregion

#region Properties

        private DataGrid OwningDataGrid
        {
            get
            {
                DataGridAutomationPeer gridPeer = _dataGridAutomationPeer as DataGridAutomationPeer;
                return (DataGrid)gridPeer.Owner;
            }
        }

        private DataGridRow OwningRow
        {
            get
            {
                int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
                int slot = this.OwningDataGrid.SlotFromRowIndex(index);
                if (this.OwningDataGrid.IsSlotVisible(slot))
                {
                    return this.OwningDataGrid.DisplayData.GetDisplayedElement(slot) as DataGridRow;
                }
                return null;

            }
        }

        internal DataGridRowAutomationPeer OwningRowPeer
        {
            get
            {
                DataGridRowAutomationPeer rowPeer = null;
                DataGridRow row = this.OwningRow;
                if (row != null)
                {
                    rowPeer = FrameworkElementAutomationPeer.CreatePeerForElement(row) as DataGridRowAutomationPeer;
                }
                return rowPeer;
            }
        }

#endregion

#region AutomationPeer Overrides

        ///
        protected override string GetAcceleratorKeyCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetAcceleratorKey() : string.Empty;
        }

        ///
        protected override string GetAccessKeyCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetAccessKey() : string.Empty;
        }

        ///
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.DataItem;
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
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetBoundingRectangle() : new Rect();
        }

        ///
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required method signature for automation peers")]
        protected override List<AutomationPeer> GetChildrenCore()
        {
            if (this.OwningRowPeer != null)
            {
                this.OwningRowPeer.InvalidatePeer();
                return this.OwningRowPeer.GetChildren();
            }
            return new List<AutomationPeer>();
        }

        ///
        protected override string GetClassNameCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetClassName() : string.Empty;
        }

        ///
        protected override Point GetClickablePointCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetClickablePoint() : new Point(double.NaN, double.NaN);
        }

        ///
        protected override string GetHelpTextCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetHelpText() : string.Empty;
        }

        ///
        protected override string GetItemStatusCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetItemStatus() : string.Empty;
        }

        ///
        override protected string GetItemTypeCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetItemType() : string.Empty;
        }

        ///
        protected override AutomationPeer GetLabeledByCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetLabeledBy() : null;
        }

        ///
        protected override string GetLocalizedControlTypeCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetLocalizedControlType() : string.Empty;
        }

        ///
        protected override string GetNameCore()
        {
            return _item.ToString();
        }

        ///
        protected override AutomationOrientation GetOrientationCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.GetOrientation() : AutomationOrientation.None;
        }

        ///
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Invoke:
                    {
                        if (!this.OwningDataGrid.IsReadOnly)
                        {
                            return this;
                        }
                        break;
                    }
                case PatternInterface.ScrollItem:
                    {
                        if (this.OwningDataGrid.VerticalScrollBar != null &&
                            this.OwningDataGrid.VerticalScrollBar.Maximum > 0)
                        {
                            return this;
                        }
                        break;
                    }
                case PatternInterface.Selection:
                case PatternInterface.SelectionItem:
                    return this;
            }
            return null;
        }

        ///
        protected override bool HasKeyboardFocusCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.HasKeyboardFocus() : false;
        }

        ///
        protected override bool IsContentElementCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.IsContentElement() : true;
        }

        ///
        protected override bool IsControlElementCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.IsControlElement() : true;
        }

        ///
        protected override bool IsEnabledCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.IsEnabled() : false;
        }

        ///
        protected override bool IsKeyboardFocusableCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.IsKeyboardFocusable() : false;
        }

        ///
        protected override bool IsOffscreenCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.IsOffscreen() : true;
        }

        ///
        protected override bool IsPasswordCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.IsPassword() : false;
        }

        ///
        protected override bool IsRequiredForFormCore()
        {
            return (this.OwningRowPeer != null) ? this.OwningRowPeer.IsRequiredForForm() : false;
        }

        ///
        protected override void SetFocusCore()
        {
            if (this.OwningRowPeer != null)
            {
                this.OwningRowPeer.SetFocus();
            }
        }

#endregion

#region IInvokeProvider

        void IInvokeProvider.Invoke()
        {
            EnsureEnabled();

            if (this.OwningRowPeer == null)
            {
                this.OwningDataGrid.ScrollIntoView(_item, null);
            }

            bool success = false;
            if (this.OwningRow != null)
            {
                if (this.OwningDataGrid.WaitForLostFocus(delegate { ((IInvokeProvider)this).Invoke(); }))
                {
                    return;
                }
                if (this.OwningDataGrid.EditingRow == this.OwningRow)
                {
                    success = this.OwningDataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/);
                }
                else if (this.OwningDataGrid.UpdateSelectionAndCurrency(this.OwningDataGrid.CurrentColumnIndex, this.OwningRow.Slot, DataGridSelectionAction.SelectCurrent, false))
                {
                    success = this.OwningDataGrid.BeginEdit();
                }
            }
            if (!success)
            {
                // 



                return;
            }
        }

#endregion

#region IScrollItemProvider

        void IScrollItemProvider.ScrollIntoView()
        {
            this.OwningDataGrid.ScrollIntoView(_item, null);
        }

#endregion

#region ISelectionItemProvider

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                return this.OwningDataGrid.SelectedItems.Contains(_item);
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                return ProviderFromPeer(_dataGridAutomationPeer);
            }
        }

        void ISelectionItemProvider.AddToSelection()
        {
            EnsureEnabled();

            if (this.OwningDataGrid.SelectionMode == DataGridSelectionMode.Single &&
                this.OwningDataGrid.SelectedItems.Count > 0 &&
                !this.OwningDataGrid.SelectedItems.Contains(_item))
            {
                // 



                return;
            }
            
            int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
            if (index != -1)
            {
                this.OwningDataGrid.SetRowSelection(this.OwningDataGrid.SlotFromRowIndex(index), true, false);
                return;
            }
            // 



        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            EnsureEnabled();

            int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
            if (index != -1)
            {
                bool success = true;
                if (this.OwningDataGrid.EditingRow != null && this.OwningDataGrid.EditingRow.Index == index)
                {
                    if (this.OwningDataGrid.WaitForLostFocus(delegate { ((ISelectionItemProvider)this).RemoveFromSelection(); }))
                    {
                        return;
                    }
                    success = this.OwningDataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/);
                }
                if (success)
                {
                    this.OwningDataGrid.SetRowSelection(this.OwningDataGrid.SlotFromRowIndex(index), false, false);
                    return;
                }
            }
            // 



        }

        void ISelectionItemProvider.Select()
        {
            EnsureEnabled();

            int index = this.OwningDataGrid.DataConnection.IndexOf(_item);
            if (index != -1)
            {
                bool success = true;
                if (this.OwningDataGrid.EditingRow != null && this.OwningDataGrid.EditingRow.Index != index)
                {
                    if (this.OwningDataGrid.WaitForLostFocus(delegate { ((ISelectionItemProvider)this).Select(); }))
                    {
                        return;
                    }
                    success = this.OwningDataGrid.CommitEdit(DataGridEditingUnit.Row, true /*exitEditing*/);
                }
                if (success)
                {
                    // Clear all the other selected items and select this one
                    int slot = this.OwningDataGrid.SlotFromRowIndex(index);
                    this.OwningDataGrid.UpdateSelectionAndCurrency(this.OwningDataGrid.CurrentColumnIndex, slot, DataGridSelectionAction.SelectCurrent, false);
                    return;
                }
            }
            // 



        }

#endregion

#region ISelectionProvider

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                return false;
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            if (this.OwningRow != null &&
                this.OwningDataGrid.IsSlotVisible(this.OwningRow.Slot) &&
                this.OwningDataGrid.CurrentSlot == this.OwningRow.Slot)
            {
                DataGridCell cell = this.OwningRow.Cells[this.OwningRow.OwningGrid.CurrentColumnIndex];
                AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(cell);
                if (peer != null)
                {
                    return new IRawElementProviderSimple[] { ProviderFromPeer(peer) };
                }
            }
            return null;
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
