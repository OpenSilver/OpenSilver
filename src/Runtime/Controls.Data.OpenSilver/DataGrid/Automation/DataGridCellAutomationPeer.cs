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

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.Column", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.ColumnSpan", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.ContainingGrid", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.Row", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.IGridItemProvider.RowSpan", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.IInvokeProvider.Invoke()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.IScrollItemProvider.ScrollIntoView()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.AddToSelection()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.IsSelected", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.RemoveFromSelection()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.Select()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.ISelectionItemProvider.SelectionContainer", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.ITableItemProvider.GetColumnHeaderItems()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridCellAutomationPeer.#System.Windows.Automation.Provider.ITableItemProvider.GetRowHeaderItems()", Justification = "Base functionality is available through the GetPattern method.")]

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
/// <summary>
/// AutomationPeer for DataGridCell
/// </summary>
/// <QualityBand>Mature</QualityBand>
public class DataGridCellAutomationPeer : FrameworkElementAutomationPeer,
        IGridItemProvider, IInvokeProvider, IScrollItemProvider, ISelectionItemProvider, ITableItemProvider
    {
#region Constructors

        /// <summary>
        /// AutomationPeer for DataGridCell
        /// </summary>
        /// <param name="owner">DataGridCell</param>
        public DataGridCellAutomationPeer(DataGridCell owner)
            : base(owner)
        {
        }

#endregion
        
#region Properties

        private IRawElementProviderSimple ContainingGrid
        {
            get
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningGrid);
                if (peer != null)
                {
                    return ProviderFromPeer(peer);
                }
                return null;
            }
        }

        private DataGridCell OwningCell
        {
            get
            {
                return (DataGridCell)Owner;
            }
        }

        private DataGridColumn OwningColumn
        {
            get
            {
                return this.OwningCell.OwningColumn;
            }
        }

        private DataGrid OwningGrid
        {
            get
            {
                return this.OwningCell.OwningGrid;
            }
        }

        private DataGridRow OwningRow
        {
            get
            {
                return this.OwningCell.OwningRow;
            }
        }

#endregion

#region AutomationPeer Overrides

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetNameCore()
        {
            TextBlock textBlock = this.OwningCell.Content as TextBlock;
            if (textBlock != null)
            {
                return textBlock.Text;
            }
            TextBox textBox = this.OwningCell.Content as TextBox;
            if (textBox != null)
            {
                return textBox.Text;
            }
            if (this.OwningColumn != null && this.OwningRow != null)
            {
                object cellContent = null;
                DataGridBoundColumn boundColumn = this.OwningColumn as DataGridBoundColumn;
                if (boundColumn != null && boundColumn.Binding != null)
                {
                    cellContent = boundColumn.GetCellValue(this.OwningRow.DataContext, boundColumn.Binding);
                }
                if (cellContent == null && this.OwningColumn.ClipboardContentBinding != null)
                {
                    cellContent = this.OwningColumn.GetCellValue(this.OwningRow.DataContext, this.OwningColumn.ClipboardContentBinding);
                }
                if (cellContent != null)
                {
                    string cellName = cellContent.ToString();
                    if (!string.IsNullOrEmpty(cellName))
                    {
                        return cellName;
                    }
                }
            }
            return base.GetNameCore();
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the System.Windows.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (this.OwningGrid != null)
            {
                switch (patternInterface)
                {
                    case PatternInterface.Invoke:
                        {
                            if (!this.OwningGrid.IsReadOnly &&
                                this.OwningColumn != null &&
                                !this.OwningColumn.IsReadOnly)
                            {
                                return this;
                            }
                            break;
                        }
                    case PatternInterface.ScrollItem:
                        {
                            if (this.OwningGrid.HorizontalScrollBar != null &&
                                this.OwningGrid.HorizontalScrollBar.Maximum > 0)
                            {
                                return this;
                            }
                            break;
                        }
                    case PatternInterface.GridItem:
                    case PatternInterface.SelectionItem:
                    case PatternInterface.TableItem:
                        return this;
                }
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets a value that indicates whether the element can accept keyboard focus.
        /// </summary>
        /// <returns>true if the element can accept keyboard focus; otherwise, false</returns>
        protected override bool IsKeyboardFocusableCore()
        {
            return true;
        }

#endregion

#region IGridItemProvider

        int IGridItemProvider.Column
        {
            get
            {
                int column = this.OwningCell.ColumnIndex;
                if (column >= 0 && this.OwningGrid != null && this.OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
                {
                    column--;
                }
                return column;
            }
        }

        int IGridItemProvider.ColumnSpan
        {
            get
            {
                return 1;
            }
        }

        IRawElementProviderSimple IGridItemProvider.ContainingGrid
        {
            get
            {
                return this.ContainingGrid;
            }
        }

        int IGridItemProvider.Row
        {
            get
            {
                return this.OwningCell.RowIndex;
            }
        }

        int IGridItemProvider.RowSpan
        {
            get
            {
                return 1;
            }
        }

#endregion

#region IInvokeProvider

        void IInvokeProvider.Invoke()
        {
            EnsureEnabled();

            if (this.OwningGrid != null)
            {
                if (this.OwningGrid.WaitForLostFocus(delegate { ((IInvokeProvider)this).Invoke(); }))
                {
                    return;
                }
                if (this.OwningGrid.EditingRow == this.OwningRow && this.OwningGrid.EditingColumnIndex == this.OwningCell.ColumnIndex)
                {
                    this.OwningGrid.CommitEdit(DataGridEditingUnit.Cell, true /*exitEditingMode*/);
                }
                else if (this.OwningGrid.UpdateSelectionAndCurrency(this.OwningCell.ColumnIndex, this.OwningRow.Slot, DataGridSelectionAction.SelectCurrent, true))
                {
                    this.OwningGrid.BeginEdit();
                }
            }
        }

#endregion

#region IScrollItemProvider

        void IScrollItemProvider.ScrollIntoView()
        {
            if (this.OwningGrid != null)
            {
                this.OwningGrid.ScrollIntoView(this.OwningCell.DataContext, this.OwningColumn);
            }
            // 






        }

#endregion

#region ISelectionItemProvider

        bool ISelectionItemProvider.IsSelected
        {
            get
            {
                if (this.OwningGrid != null)
                {
                    return (this.OwningGrid.CurrentSlot == this.OwningRow.Slot && this.OwningGrid.CurrentColumnIndex == this.OwningCell.ColumnIndex);
                }
                // 



                return false;
            }
        }

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningRow);
                if (peer != null)
                {
                    return ProviderFromPeer(peer);
                }
                return null;
            }
        }

        void ISelectionItemProvider.AddToSelection()
        {
            EnsureEnabled();
            // 









        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            EnsureEnabled();
            // 









        }

        void ISelectionItemProvider.Select()
        {
            EnsureEnabled();

            if (this.OwningGrid != null)
            {
                if (this.OwningGrid.WaitForLostFocus(delegate { ((ISelectionItemProvider)this).Select(); }))
                {
                    return;
                }
                this.OwningGrid.UpdateSelectionAndCurrency(this.OwningCell.ColumnIndex, this.OwningRow.Slot, DataGridSelectionAction.SelectCurrent, false);
            }
        }

#endregion

#region ITableItemProvider

        IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
        {
            if (this.OwningGrid != null &&
                this.OwningGrid.AreColumnHeadersVisible &&
                this.OwningColumn.HeaderCell != null)
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningColumn.HeaderCell);
                if (peer != null)
                {
                    List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>(1);
                    providers.Add(ProviderFromPeer(peer));
                    return providers.ToArray();
                }
            }
            return null;
        }

        IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
        {
            if (this.OwningGrid != null && 
                this.OwningGrid.AreRowHeadersVisible &&
                this.OwningRow.HeaderCell != null)
            {
                AutomationPeer peer = CreatePeerForElement(this.OwningRow.HeaderCell);
                if (peer != null)
                {
                    List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>(1);
                    providers.Add(ProviderFromPeer(peer));
                    return providers.ToArray();
                }
            }
            return null;
        }

#endregion

#region Private Methods

        private void EnsureEnabled()
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

#endregion
    }
}
