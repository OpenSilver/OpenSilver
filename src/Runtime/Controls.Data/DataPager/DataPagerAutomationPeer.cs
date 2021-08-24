//-----------------------------------------------------------------------
// <copyright file="DataPagerAutomationPeer.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Globalization;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Automation peer for the <see cref="T:System.Windows.Controls.DataPager" /> control.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DataPagerAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        #region Data

        private bool _isReadOnly;
        private int _largeChange;
        private int _minimum, _maximum;

        #endregion Data

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Automation.Peers.DataPagerAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="T:System.Windows.Controls.DataPager" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.DataPagerAutomationPeer" />.
        /// </param>
        public DataPagerAutomationPeer(DataPager owner)
            : base(owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            this.InitializeProperties();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="T:System.Windows.Controls.DataPager" /> control allows page changes.
        /// </summary>
        private bool IsReadOnlyPrivate
        {
            get
            {
                return this._isReadOnly;
            }

            set
            {
                if (value != this._isReadOnly)
                {
                    this._isReadOnly = value;
                    this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.IsReadOnlyProperty, !this._isReadOnly, this._isReadOnly);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value to be added or subtracted from the page number of the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        private double LargeChangePrivate
        {
            get
            {
                return this._largeChange;
            }

            set
            {
                if (value != this._largeChange)
                {
                    double oldLargeChange = this._largeChange;
                    this._largeChange = (int)value;
                    this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.LargeChangeProperty, oldLargeChange, this._largeChange);
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum page number value for the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        private double MaximumPrivate
        {
            get
            {
                return this._maximum;
            }

            set
            {
                if (value != this._maximum)
                {
                    double oldMaximum = this._maximum;
                    this._maximum = (int)value;
                    this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MaximumProperty, oldMaximum, this._maximum);
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum page number value for the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        private double MinimumPrivate
        {
            get
            {
                return this._minimum;
            }

            set
            {
                if (value != this._minimum)
                {
                    double oldMinimum = this._minimum;
                    this._minimum = (int)value;
                    this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.MinimumProperty, oldMinimum, this._minimum);
                }
            }
        }

        /// <summary>
        /// Gets the owner as a DataPager
        /// </summary>
        private DataPager OwningDataPager
        {
            get
            {
                return this.Owner as DataPager;
            }
        }

        #endregion Properties

        #region AutomationPeer Overrides

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <summary>
        /// Gets text that describes the DataPager that is associated with this automation peer.
        /// Called by System.Windows.Automation.Peers.AutomationPeer.GetName().
        /// </summary>
        /// <returns>
        /// When the control is not paging data:
        /// - Value returned by the base implementation if it's not empty
        /// - Name of the owning DataPager control if it's not empty
        /// - Name returned by the LabeledBy automation peer if it's not empty
        /// - DataPager class name if none of the above is valid
        /// When the control is paging data:
        /// - "Page N" when the total page count is unknown
        /// - "Page N of M" when the count is known
        /// </returns>
        protected override string GetNameCore()
        {
            if (this.OwningDataPager.Source == null || this.OwningDataPager.PageSize == 0)
            {
                string name = base.GetNameCore();
                if (string.IsNullOrEmpty(name))
                {
                    name = this.OwningDataPager.Name;
                }
                if (string.IsNullOrEmpty(name))
                {
                    AutomationPeer labeledByAutomationPeer = this.GetLabeledByCore();
                    if (labeledByAutomationPeer != null)
                    {
                        name = labeledByAutomationPeer.GetName();
                    }
                }
                if (string.IsNullOrEmpty(name))
                {
                    name = this.GetClassNameCore();
                }
                return name;
            }
            if (this.OwningDataPager.PagedSource != null &&
                this.OwningDataPager.PagedSource.TotalItemCount == -1)
            {
                // Returns "Page M"
                return string.Format(
                    CultureInfo.InvariantCulture,
                    PagerResources.AutomationPeerName_TotalPageCountUnknown,
                    (this.OwningDataPager.PageIndex + 1).ToString(CultureInfo.CurrentCulture));
            }
            // Returns "Page M of N"
            return string.Format(
                CultureInfo.InvariantCulture,
                PagerResources.AutomationPeerName_TotalPageCountKnown,
                (this.OwningDataPager.PageIndex + 1).ToString(CultureInfo.CurrentCulture),
                this.OwningDataPager.PageCount.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the System.Windows.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.RangeValue ? this : base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Returns a System.Windows.Point that represents the clickable space that is
        /// on the System.Windows.UIElement that is associated with this System.Windows.Automation.Peers.FrameworkElementAutomationPeer.
        /// This method is called by System.Windows.Automation.Peers.AutomationPeer.GetClickablePoint().
        /// </summary>
        /// <returns>The System.Windows.Point on the element that allows a click.</returns>
        protected override Point GetClickablePointCore()
        {
            // Return the clickable point of the page number TextBox when present.
            switch (this.OwningDataPager.DisplayMode)
            {
                case PagerDisplayMode.FirstLastPreviousNext:
                case PagerDisplayMode.PreviousNext:
                    if (this.OwningDataPager.CurrentPageTextBox != null)
                    {
                        AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this.OwningDataPager.CurrentPageTextBox);
                        if (peer != null)
                        {
                            peer.InvalidatePeer();
                        }
                        else
                        {
                            peer = FrameworkElementAutomationPeer.CreatePeerForElement(this.OwningDataPager.CurrentPageTextBox);
                        }
                        if (peer != null)
                        {
                            return peer.GetClickablePoint();
                        }
                    }
                    break;
            }
            return base.GetClickablePointCore();
        }

        #endregion AutomationPeer Overrides

        #region IRangeValueProvider Members

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Windows.Controls.DataPager" /> control allows page changes.
        /// </summary>
        bool IRangeValueProvider.IsReadOnly
        {
            get
            {
                return this.IsReadOnlyPrivate;
            }
        }

        /// <summary>
        /// Gets the value to be added or subtracted from the page number of the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        double IRangeValueProvider.LargeChange
        {
            get
            {
                return this.LargeChangePrivate;
            }
        }

        /// <summary>
        /// Gets the maximum page number value for the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        double IRangeValueProvider.Maximum
        {
            get
            {
                return this.MaximumPrivate;
            }
        }

        /// <summary>
        /// Gets the minimum page number value for the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        double IRangeValueProvider.Minimum
        {
            get
            {
                return this.MinimumPrivate;
            }
        }

        /// <summary>
        /// Sets the page number of the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        /// <param name="value">Value used for setting the PageIndex property of the <see cref="T:System.Windows.Controls.DataPager" /> control</param>
        void IRangeValueProvider.SetValue(double value)
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            int pageIndex = (int)value;
            if (pageIndex > 0)
            {
                pageIndex--;
            }
            else
            {
                pageIndex = -1;
            }
            this.OwningDataPager.PageIndex = pageIndex;
        }

        /// <summary>
        /// Gets the value to be added or subtracted from the page number of the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        double IRangeValueProvider.SmallChange
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the page number of the <see cref="T:System.Windows.Controls.DataPager" /> control.
        /// </summary>
        double IRangeValueProvider.Value
        {
            get
            {
                return this.OwningDataPager.PageIndex == -1 ? -1 : this.OwningDataPager.PageIndex + 1;
            }
        }

        #endregion IRangeValueProvider Members

        #region Methods

        /// <summary>
        /// Computes the value for IRangeValueProvider's IsReadOnly property 
        /// </summary>
        /// <returns>True when no page move is allowed</returns>
        private bool GetIsReadOnlyCore()
        {
            return !this.OwningDataPager.CanChangePage || !this.IsEnabled();
        }

        /// <summary>
        /// Computes the value for IRangeValueProvider's LargeChange property 
        /// </summary>
        /// <returns>Large change value to use</returns>
        private int GetLargeChangeCore()
        {
            if (this.OwningDataPager.Source == null || this.OwningDataPager.PageSize == 0)
            {
                return 1;
            }

            return this.OwningDataPager.PageCount;
        }

        /// <summary>
        /// Computes the value for IRangeValueProvider's Maximum property 
        /// </summary>
        /// <returns>Maximum value to use</returns>
        private int GetMaximumCore()
        {
            if (this.OwningDataPager.Source == null || this.OwningDataPager.PageSize == 0)
            {
                return -1;
            }

            if (this.OwningDataPager.PagedSource == null ||
                (this.OwningDataPager.IsTotalItemCountFixed && this.OwningDataPager.PagedSource.TotalItemCount != -1))
            {
                return this.OwningDataPager.PageCount;
            }

            return this.OwningDataPager.PageCount + 1;
        }

        /// <summary>
        /// Computes the value for IRangeValueProvider's Minimum property 
        /// </summary>
        /// <returns>Minimum value to use</returns>
        private int GetMinimumCore()
        {
            if (this.OwningDataPager.Source == null || this.OwningDataPager.PageSize == 0)
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Initializes all the variable IRangeValueProvider properties
        /// </summary>
        private void InitializeProperties()
        {
            this._isReadOnly = this.GetIsReadOnlyCore();
            this._largeChange = this.GetLargeChangeCore();
            this._maximum = this.GetMaximumCore();
            this._minimum = this.GetMinimumCore();
        }

        /// <summary>
        /// Method called by the owning DataPager when its page index has changed.
        /// </summary>
        /// <param name="oldPageIndex">Previous value of the PageIndex property</param>
        internal void RefreshPageIndex(int oldPageIndex)
        {
            if (oldPageIndex != -1)
            {
                oldPageIndex++;
            }
            this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldPageIndex, this.OwningDataPager.PageIndex + 1);
        }

        /// <summary>
        /// Method called by the owning DataPager when the IRangeValueProvider properties need to be recomputed.
        /// </summary>
        internal void RefreshProperties()
        {
            this.IsReadOnlyPrivate = this.GetIsReadOnlyCore();
            this.LargeChangePrivate = this.GetLargeChangeCore();
            this.MaximumPrivate = this.GetMaximumCore();
            this.MinimumPrivate = this.GetMinimumCore();
        }

        #endregion Methods
    }
}
