// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;


#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.IInvokeProvider.Invoke()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.IScrollItemProvider.ScrollIntoView()", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanMove", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanResize", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanRotate", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Move(System.Double,System.Double)", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Resize(System.Double,System.Double)", Justification = "Base functionality is available through the GetPattern method.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Windows.Automation.Peers.DataGridColumnHeaderAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Rotate(System.Double)", Justification = "Base functionality is available through the GetPattern method.")]

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// AutomationPeer for DataGridColumnHeader
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridColumnHeaderAutomationPeer : FrameworkElementAutomationPeer,
        IInvokeProvider, IScrollItemProvider, ITransformProvider
    {
        #region Constructors

        /// <summary>
        /// AutomationPeer for DataGridColumnHeader
        /// </summary>
        /// <param name="owner">DataGridColumnHeader</param>
        public DataGridColumnHeaderAutomationPeer(DataGridColumnHeader owner)
            : base(owner)
        {
        }

        #endregion

        #region Properties

        private DataGridColumnHeader OwningHeader
        {
            get
            {
                return (DataGridColumnHeader)Owner;
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
            return AutomationControlType.HeaderItem;
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
        /// Gets the string that describes the functionality of the control that is associated with the automation peer. 
        /// </summary>
        /// <returns>The string that contains the help text.</returns>
        protected override string GetHelpTextCore()
        {
            if (this.OwningHeader.CurrentSortingState.HasValue)
            {
                if (this.OwningHeader.CurrentSortingState.Value == ListSortDirection.Ascending)
                {
                    return Resource.DataGridColumnHeaderAutomationPeer_Ascending;
                }
                return Resource.DataGridColumnHeaderAutomationPeer_Descending;
            }
            return base.GetHelpTextCore();
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetNameCore()
        {
            string header = this.OwningHeader.Content as string;
            if (header != null)
            {
                return header;
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
            if (this.OwningHeader.OwningGrid != null)
            {
                switch (patternInterface)
                {
                    case PatternInterface.Invoke:
                        {
                            if (this.OwningHeader.OwningGrid.DataConnection.AllowSort &&
                                this.OwningHeader.OwningGrid.CanUserSortColumns &&
                                this.OwningHeader.OwningColumn.CanUserSort)
                            {
                                return this;
                            }
                            break;
                        }
                    case PatternInterface.ScrollItem:
                        {
                            if (this.OwningHeader.OwningGrid.HorizontalScrollBar != null &&
                                this.OwningHeader.OwningGrid.HorizontalScrollBar.Maximum > 0)
                            {
                                return this;
                            }
                            break;
                        }
                    case PatternInterface.Transform:
                        {
                            if (this.OwningHeader.OwningColumn != null &&
                                this.OwningHeader.OwningColumn.ActualCanUserResize)
                            {
                                return this;
                            }
                            break;
                        }

                }
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets a value that specifies whether the element is a content element.
        /// </summary>
        /// <returns>true if the element is a content element; otherwise false</returns>
        protected override bool IsContentElementCore()
        {
            return false;
        }

        #endregion

        #region IInvokeProvider

        void IInvokeProvider.Invoke()
        {
            this.OwningHeader.InvokeProcessSort();
        }

        #endregion

        #region IScrollItemProvider

        void IScrollItemProvider.ScrollIntoView()
        {
            this.OwningHeader.OwningGrid.ScrollIntoView(null, this.OwningHeader.OwningColumn);
        }

        #endregion

        #region ITransformProvider

        bool ITransformProvider.CanMove { get { return false; } }

        bool ITransformProvider.CanResize
        {
            get
            {
                return this.OwningHeader.OwningColumn != null && this.OwningHeader.OwningColumn.ActualCanUserResize;
            }
        }

        bool ITransformProvider.CanRotate { get { return false; } }

        void ITransformProvider.Move(double x, double y)
        {
            // 



        }

        void ITransformProvider.Resize(double width, double height)
        {
            if (this.OwningHeader.OwningColumn != null &&
                this.OwningHeader.OwningColumn.ActualCanUserResize)
            {
                this.OwningHeader.OwningColumn.Width = new DataGridLength(width);
            }
        }

        void ITransformProvider.Rotate(double degrees)
        {
            // 



        }

        #endregion
    }
}
