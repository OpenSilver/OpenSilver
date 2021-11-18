//-----------------------------------------------------------------------
// <copyright file="DataPager.cs" company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

#if MIGRATION
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Common;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Handles paging for an <see cref="T:System.ComponentModel.IPagedCollectionView" />. 
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = DATAPAGER_elementFirstPageButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAPAGER_elementPreviousPageButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAPAGER_elementCurrentPagePrefixTextBlockn, Type = typeof(TextBlock))]
    [TemplatePart(Name = DATAPAGER_elementCurrentPageSuffixTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = DATAPAGER_elementCurrentPageTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = DATAPAGER_elementNextPageButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAPAGER_elementLastPageButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAPAGER_elementNumericButtonPanel, Type = typeof(Panel))]
    [TemplateVisualState(Name = DATAPAGER_stateNormal, GroupName = DATAPAGER_groupCommon)]
    [TemplateVisualState(Name = DATAPAGER_stateDisabled, GroupName = DATAPAGER_groupCommon)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveEnabled, GroupName = DATAPAGER_groupMove)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveDisabled, GroupName = DATAPAGER_groupMove)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveFirstEnabled, GroupName = DATAPAGER_groupMoveFirst)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveFirstDisabled, GroupName = DATAPAGER_groupMoveFirst)]
    [TemplateVisualState(Name = DATAPAGER_stateMovePreviousEnabled, GroupName = DATAPAGER_groupMovePrevious)]
    [TemplateVisualState(Name = DATAPAGER_stateMovePreviousDisabled, GroupName = DATAPAGER_groupMovePrevious)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveNextEnabled, GroupName = DATAPAGER_groupMoveNext)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveNextDisabled, GroupName = DATAPAGER_groupMoveNext)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveLastEnabled, GroupName = DATAPAGER_groupMoveLast)]
    [TemplateVisualState(Name = DATAPAGER_stateMoveLastDisabled, GroupName = DATAPAGER_groupMoveLast)]
    [TemplateVisualState(Name = DATAPAGER_stateTotalPageCountKnown, GroupName = DATAPAGER_groupTotalPageCountKnown)]
    [TemplateVisualState(Name = DATAPAGER_stateTotalPageCountUnknown, GroupName = DATAPAGER_groupTotalPageCountKnown)]
    [TemplateVisualState(Name = DATAPAGER_stateFirstLastNumeric, GroupName = DATAPAGER_groupDisplayMode)]
    [TemplateVisualState(Name = DATAPAGER_stateFirstLastPreviousNext, GroupName = DATAPAGER_groupDisplayMode)]
    [TemplateVisualState(Name = DATAPAGER_stateFirstLastPreviousNextNumeric, GroupName = DATAPAGER_groupDisplayMode)]
    [TemplateVisualState(Name = DATAPAGER_stateNumeric, GroupName = DATAPAGER_groupDisplayMode)]
    [TemplateVisualState(Name = DATAPAGER_statePreviousNext, GroupName = DATAPAGER_groupDisplayMode)]
    [TemplateVisualState(Name = DATAPAGER_statePreviousNextNumeric, GroupName = DATAPAGER_groupDisplayMode)]
    [StyleTypedProperty(Property = DATAPAGER_styleNumericButton, StyleTargetType = typeof(ToggleButton))]
    public class DataPager : Control
    {
        ////------------------------------------------------------
        ////
        ////  Static Fields and Constants
        ////
        ////------------------------------------------------------ 

#region Constants

        // Automation Id constants
        private const string DATAPAGER_currentPageTextBoxAutomationId = "CurrentPage";
        private const string DATAPAGER_firstPageButtonAutomationId = "LargeDecrement";
        private const string DATAPAGER_lastPageButtonAutomationId = "LargeIncrement";
        private const string DATAPAGER_nextPageButtonAutomationId = "SmallIncrement";
        private const string DATAPAGER_numericalButtonAutomationId = "MoveToPage";
        private const string DATAPAGER_previousPageButtonAutomationId = "SmallDecrement";

        // Parts constants
        private const string DATAPAGER_elementCurrentPagePrefixTextBlockn = "CurrentPagePrefixTextBlock";
        private const string DATAPAGER_elementCurrentPageSuffixTextBlock = "CurrentPageSuffixTextBlock";
        private const string DATAPAGER_elementCurrentPageTextBox = "CurrentPageTextBox";
        private const string DATAPAGER_elementFirstPageButton = "FirstPageButton";
        private const string DATAPAGER_elementLastPageButton = "LastPageButton";
        private const string DATAPAGER_elementNextPageButton = "NextPageButton";
        private const string DATAPAGER_elementNumericButtonPanel = "NumericButtonPanel";
        private const string DATAPAGER_elementPreviousPageButton = "PreviousPageButton";

        // Styles constants
        private const string DATAPAGER_styleNumericButton = "NumericButtonStyle";

        // Common states constants        
        private const string DATAPAGER_groupCommon = "CommonStates";
        private const string DATAPAGER_stateNormal = "Normal";
        private const string DATAPAGER_stateDisabled = "Disabled";

        // Move states constants        
        private const string DATAPAGER_groupMove = "MoveStates";
        private const string DATAPAGER_stateMoveEnabled = "MoveEnabled";
        private const string DATAPAGER_stateMoveDisabled = "MoveDisabled";

        // MoveFirst states constants        
        private const string DATAPAGER_groupMoveFirst = "MoveFirstStates";
        private const string DATAPAGER_stateMoveFirstEnabled = "MoveFirstEnabled";
        private const string DATAPAGER_stateMoveFirstDisabled = "MoveFirstDisabled";

        // MovePrevious states constants        
        private const string DATAPAGER_groupMovePrevious = "MovePreviousStates";
        private const string DATAPAGER_stateMovePreviousEnabled = "MovePreviousEnabled";
        private const string DATAPAGER_stateMovePreviousDisabled = "MovePreviousDisabled";

        // MovePrevious states constants        
        private const string DATAPAGER_groupMoveNext = "MoveNextStates";
        private const string DATAPAGER_stateMoveNextEnabled = "MoveNextEnabled";
        private const string DATAPAGER_stateMoveNextDisabled = "MoveNextDisabled";

        // MovePrevious states constants        
        private const string DATAPAGER_groupMoveLast = "MoveLastStates";
        private const string DATAPAGER_stateMoveLastEnabled = "MoveLastEnabled";
        private const string DATAPAGER_stateMoveLastDisabled = "MoveLastDisabled";

        // TotalPageCountKnown states constants        
        private const string DATAPAGER_groupTotalPageCountKnown = "TotalPageCountKnownStates";
        private const string DATAPAGER_stateTotalPageCountKnown = "TotalPageCountKnown";
        private const string DATAPAGER_stateTotalPageCountUnknown = "TotalPageCountUnknown";

        // DisplayModeStates states constants
        private const string DATAPAGER_groupDisplayMode = "DisplayModeStates";
        private const string DATAPAGER_stateFirstLastNumeric = "FirstLastNumeric";
        private const string DATAPAGER_stateFirstLastPreviousNext = "FirstLastPreviousNext";
        private const string DATAPAGER_stateFirstLastPreviousNextNumeric = "FirstLastPreviousNextNumeric";
        private const string DATAPAGER_stateNumeric = "Numeric";
        private const string DATAPAGER_statePreviousNext = "PreviousNext";
        private const string DATAPAGER_statePreviousNextNumeric = "PreviousNextNumeric";

        // Default property value constants
        private const PagerDisplayMode DATAPAGER_defaultDisplayMode = PagerDisplayMode.FirstLastPreviousNext;
        private const int DATAPAGER_defaultNumericButtonCount = 5;
        private const int DATAPAGER_defaultPageIndex = -1;

#endregion Constants

#region Static Fields

        /// <summary>
        /// Identifies the AutoEllipsis dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoEllipsisProperty =
            DependencyProperty.Register(
                "AutoEllipsis",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnAutoEllipsisPropertyChanged));

        /// <summary>
        /// Identifies the CanChangePage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanChangePageProperty =
            DependencyProperty.Register(
                "CanChangePage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToFirstPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToFirstPageProperty =
            DependencyProperty.Register(
                "CanMoveToFirstPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToLastPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToLastPageProperty =
            DependencyProperty.Register(
                "CanMoveToLastPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToNextPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToNextPageProperty =
            DependencyProperty.Register(
                "CanMoveToNextPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the CanMoveToPreviousPage dependency property.
        /// </summary>
        public static readonly DependencyProperty CanMoveToPreviousPageProperty =
            DependencyProperty.Register(
                "CanMoveToPreviousPage",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the DisplayMode dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
                "DisplayMode",
                typeof(PagerDisplayMode),
                typeof(DataPager),
                new PropertyMetadata(DATAPAGER_defaultDisplayMode, OnDisplayModePropertyChanged));

        /// <summary>
        /// Identifies the IsTotalItemCountFixed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTotalItemCountFixedProperty =
            DependencyProperty.Register(
                "IsTotalItemCountFixed",
                typeof(bool),
                typeof(DataPager),
                new PropertyMetadata(OnIsTotalItemCountFixedPropertyChanged));

        /// <summary>
        /// Identifies the ItemCount dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register(
                "ItemCount",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));

        /// <summary>
        /// Identifies the NumericButtonCount dependency property.
        /// </summary>
        public static readonly DependencyProperty NumericButtonCountProperty =
            DependencyProperty.Register(
                "NumericButtonCount",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(DATAPAGER_defaultNumericButtonCount, OnNumericButtonCountPropertyChanged));

        /// <summary>
        /// Identifies the NumericButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty NumericButtonStyleProperty =
            DependencyProperty.Register(
                "NumericButtonStyle",
                typeof(Style),
                typeof(DataPager),
                new PropertyMetadata(OnNumericButtonStylePropertyChanged));

        /// <summary>
        /// Identifies the PageCount dependency property.
        /// </summary>
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register(
                "PageCount",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(OnReadOnlyPropertyChanged));
        
        /// <summary>
        /// Identifies the PageIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register(
                "PageIndex",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(DATAPAGER_defaultPageIndex, OnPageIndexPropertyChanged));

        /// <summary>
        /// Identifies the PageSize dependency property.
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(
                "PageSize",
                typeof(int),
                typeof(DataPager),
                new PropertyMetadata(OnPageSizePropertyChanged));

        /// <summary>
        /// Identifies the PrivateForeground dependency property. This dependency property is bound to 
        /// DataPager.Foreground to be aware of its changes. It is used to update the foreground of the
        /// numeric toggle buttons.
        /// </summary>
        private static readonly DependencyProperty PrivateForegroundProperty =
            DependencyProperty.Register(
                "PrivateForeground",
                typeof(Brush),
                typeof(DataPager),
                new PropertyMetadata(OnPrivateForegroundPropertyChanged));

        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(IEnumerable),
                typeof(DataPager),
                new PropertyMetadata(OnSourcePropertyChanged));

#endregion Static Fields

        ////------------------------------------------------------
        ////
        ////  Private Fields
        ////
        ////------------------------------------------------------ 

#region Private Fields

        /// <summary>
        /// Private accessor for the text block appearing before the current page text box.
        /// </summary>
        private TextBlock _currentPagePrefixTextBlock;

        /// <summary>
        /// Private accessor for the text block appearing after the current page text box.
        /// </summary>
        private TextBlock _currentPageSuffixTextBlock;

        /// <summary>
        /// Private accessor for the current page text box.
        /// </summary>
        private TextBox _currentPageTextBox;

        /// <summary>
        /// Private accessor for the first page ButtonBase.
        /// </summary>
        private ButtonBase _firstPageButtonBase;

        /// <summary>
        /// Page index corresponding to the ToggleButton that has keyboard focus
        /// -1 if no ToggleButton has focus.
        /// </summary>
        private int _focusedToggleButtonIndex = -1;

        /// <summary>
        /// Set to True when the ToggleButton_Checked notification needs to be ignored.
        /// </summary>
        private bool _ignoreToggleButtonCheckedNotification;

        /// <summary>
        /// Set to True when the ToggleButton_GotFocus and ToggleButton_LostFocus 
        /// notifications need to be ignored.
        /// </summary>
        private bool _ignoreToggleButtonFocusNotification;

        /// <summary>
        /// Set to True when a ToggleButton_Unchecked notification needs to be ignored.
        /// </summary>
        private bool _ignoreToggleButtonUncheckedNotification;

        /// <summary>
        /// Private accessor for the last page ButtonBase.
        /// </summary>
        private ButtonBase _lastPageButtonBase;

        /// <summary>
        /// Set to True when a PageChanging notification is expected to be raised
        /// before the next PagedChanged notification.
        /// </summary>
        private bool _needPageChangingNotification = true;

        /// <summary>
        /// Private accessor for the next page ButtonBase.
        /// </summary>
        private ButtonBase _nextPageButtonBase;

        /// <summary>
        /// Private accessor for the panel hosting the buttons.
        /// </summary>
        private Panel _numericButtonPanel;

        /// <summary>
        /// Private accessor for the previous page ButtonBase.
        /// </summary>
        private ButtonBase _previousPageButtonBase;

        /// <summary>
        /// The new index of the current page, used to change the
        /// current page when a user enters something into the
        /// current page text box.
        /// </summary>
        private int _requestedPageIndex;

        /// <summary>
        /// Holds the weak event listener for the INotifyPropertyChanged.PropertyChanged event.
        /// </summary>
        private WeakEventListener<DataPager, object, PropertyChangedEventArgs> _weakEventListenerPropertyChanged;

        /// <summary>
        /// Delegate for calling page move operations
        /// </summary>
        /// <returns>Boolean value for whether the operation succeeded</returns>
        private delegate bool PageMoveOperationDelegate();

#endregion Private Fields

        ////------------------------------------------------------
        ////
        ////  Constructors
        ////
        ////------------------------------------------------------ 

#region Constructors

        /// <summary>
        /// Initializes a new instance of the DataPager class.
        /// </summary>
        public DataPager()
        {
            CustomLayout = true;
            this.DefaultStyleKey = typeof(DataPager);

            // Listening to the IsEnabled changes so the DataPager states can be updated accordingly.
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnDataPagerIsEnabledChanged);

            // Binding the DataPager.Foreground property, one way, to the PrivateForeground property
            Binding foregroundBinding = new Binding("Foreground");
            foregroundBinding.Source = this;
            this.SetBinding(DataPager.PrivateForegroundProperty, foregroundBinding);
        }

#endregion Constructors

        ////------------------------------------------------------
        ////
        ////  Events
        ////
        ////------------------------------------------------------ 

#region Events

        /// <summary>
        /// EventHandler for when PageIndex is changing.
        /// </summary>
        public event EventHandler<CancelEventArgs> PageIndexChanging;

        /// <summary>
        /// EventHandler for when PageIndex has changed.
        /// </summary>
        public event EventHandler<EventArgs> PageIndexChanged;

#endregion Events

        ////------------------------------------------------------
        ////
        ////  Public Properties
        ////
        ////------------------------------------------------------ 

#region Public Properties

        /// <summary>
        /// Gets or sets a value that indicates whether or not to use an ellipsis as the last button.
        /// </summary>
        public bool AutoEllipsis
        {
            get
            {
                return (bool)GetValue(AutoEllipsisProperty);
            }

            set
            {
                SetValue(AutoEllipsisProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the user is allowed to move to another page
        /// </summary>
        public bool CanChangePage
        {
            get
            {
                return (bool)GetValue(CanChangePageProperty);
            }

            private set
            {
                this.SetValueNoCallback(CanChangePageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the <see cref="T:System.Windows.Controls.DataPager" /> will 
        /// allow the user to attempt to move to the first page if <see cref="P:System.Windows.Controls.DataPager.CanChangePage" /> is true. 
        /// </summary>
        public bool CanMoveToFirstPage
        {
            get
            {
                return (bool)GetValue(CanMoveToFirstPageProperty);
            }

            private set
            {
                this.SetValueNoCallback(CanMoveToFirstPageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the <see cref="T:System.Windows.Controls.DataPager" /> 
        /// will allow the user to attempt to move to the last page if <see cref="P:System.Windows.Controls.DataPager.CanChangePage" /> is true. 
        /// </summary>
        public bool CanMoveToLastPage
        {
            get
            {
                return (bool)GetValue(CanMoveToLastPageProperty);
            }

            private set
            {
                this.SetValueNoCallback(CanMoveToLastPageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the <see cref="T:System.Windows.Controls.DataPager" /> 
        /// will allow the user to attempt to move to the next page if<see cref="P:System.Windows.Controls.DataPager.CanChangePage" /> is true. 
        /// </summary>
        public bool CanMoveToNextPage
        {
            get
            {
                return (bool)GetValue(CanMoveToNextPageProperty);
            }

            private set
            {
                this.SetValueNoCallback(CanMoveToNextPageProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the <see cref="T:System.Windows.Controls.DataPager" /> 
        /// will allow the user to attempt to move to the previous page if <see cref="P:System.Windows.Controls.DataPager.CanChangePage" /> is true.
        /// </summary>
        public bool CanMoveToPreviousPage
        {
            get
            {
                return (bool)GetValue(CanMoveToPreviousPageProperty);
            }

            private set
            {
                this.SetValueNoCallback(CanMoveToPreviousPageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates how the 
        /// <see cref="T:System.Windows.Controls.DataPager" /> user interface is displayed
        /// </summary>
        public PagerDisplayMode DisplayMode
        {
            get
            {
                return (PagerDisplayMode)GetValue(DisplayModeProperty);
            }

            set
            {
                SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether or not the total number of items in the collection is fixed.
        /// </summary>
        public bool IsTotalItemCountFixed
        {
            get
            {
                return (bool)GetValue(IsTotalItemCountFixedProperty);
            }

            set
            {
                SetValue(IsTotalItemCountFixedProperty, value);
            }
        }

        /// <summary>
        /// Gets the current number of known items in the <see cref="T:System.ComponentModel.IPagedCollectionView" /> . 
        /// </summary>
        public int ItemCount
        {
            get
            {
                return (int)GetValue(ItemCountProperty);
            }

            private set
            {
                this.SetValueNoCallback(ItemCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.ComponentModel.IPagedCollectionView" /> . 
        /// </summary>
        public IEnumerable Source
        {
            get
            {
                return GetValue(SourceProperty) as IEnumerable;
            }

            set
            {
                SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the number of page buttons shown 
        /// on the <see cref="T:System.Windows.Controls.DataPager" /> user interface. 
        /// </summary>
        public int NumericButtonCount
        {
            get
            {
                return (int)GetValue(NumericButtonCountProperty);
            }

            set
            {
                SetValue(NumericButtonCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style that will be used for the numeric buttons.
        /// </summary>
        public Style NumericButtonStyle
        {
            get
            {
                return (Style)GetValue(NumericButtonStyleProperty);
            }

            set
            {
                SetValue(NumericButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets the current number of known pages in the <see cref="T:System.ComponentModel.IPagedCollectionView" /> . 
        /// </summary>
        public int PageCount
        {
            get
            {
                return (int)GetValue(PageCountProperty);
            }

            private set
            {
                this.SetValueNoCallback(PageCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex" /> 
        /// in the <see cref="T:System.ComponentModel.IPagedCollectionView" /> . 
        /// </summary>
        [DefaultValueAttribute(-1)]
        public int PageIndex
        {
            get
            {
                return (int)GetValue(PageIndexProperty);
            }

            set
            {
                SetValue(PageIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="P:System.ComponentModel.IPagedCollectionView.PageSize" /> in the <see cref="T:System.ComponentModel.IPagedCollectionView" /> .
        /// </summary>
        public int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }

            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

#endregion Public Properties

        ////------------------------------------------------------
        ////
        ////  Internal Properties
        ////
        ////------------------------------------------------------ 

#region Internal Properties

        /// <summary>
        /// Gets the TextBox holding the current PageIndex value, if any.
        /// </summary>
        internal TextBox CurrentPageTextBox
        {
            get
            {
                return this._currentPageTextBox;
            }
        }

        /// <summary>
        /// Gets the Source as an IPagedCollectionView
        /// </summary>
        internal IPagedCollectionView PagedSource
        {
            get
            {
                return this.Source as IPagedCollectionView;
            }
        }

#endregion Internal Properties


        /// <summary>
        /// Gets or sets the items to page through.
        /// </summary>
        private Brush PrivateForeground
        {
            get
            {
                return GetValue(PrivateForegroundProperty) as Brush;
            }

            set
            {
                SetValue(PrivateForegroundProperty, value);
            }
        }

        ////------------------------------------------------------
        ////
        ////  Public Methods
        ////
        ////------------------------------------------------------ 

#region Public Methods

        /// <summary>
        /// Applies the control's template, retrieves the elements
        /// within it, and sets up events.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // unsubscribe event handlers for previous template parts
            if (this._firstPageButtonBase != null)
            {
                this._firstPageButtonBase.Click -= new RoutedEventHandler(this.OnFirstPageButtonBaseClick);
            }

            if (this._previousPageButtonBase != null)
            {
                this._previousPageButtonBase.Click -= new RoutedEventHandler(this.OnPreviousPageButtonBaseClick);
            }

            if (this._nextPageButtonBase != null)
            {
                this._nextPageButtonBase.Click -= new RoutedEventHandler(this.OnNextPageButtonBaseClick);
            }

            if (this._lastPageButtonBase != null)
            {
                this._lastPageButtonBase.Click -= new RoutedEventHandler(this.OnLastPageButtonBaseClick);
            }

            if (this._currentPageTextBox != null)
            {
                this._currentPageTextBox.KeyDown -= new System.Windows.Input.KeyEventHandler(this.OnCurrentPageTextBoxKeyDown);
                this._currentPageTextBox.LostFocus -= new RoutedEventHandler(this.OnCurrentPageTextBoxLostFocus);
            }

            // get new template parts
            this._firstPageButtonBase = GetTemplateChild(DATAPAGER_elementFirstPageButton) as ButtonBase;
            this._previousPageButtonBase = GetTemplateChild(DATAPAGER_elementPreviousPageButton) as ButtonBase;
            this._nextPageButtonBase = GetTemplateChild(DATAPAGER_elementNextPageButton) as ButtonBase;
            this._lastPageButtonBase = GetTemplateChild(DATAPAGER_elementLastPageButton) as ButtonBase;

            if (this._firstPageButtonBase != null)
            {
                this._firstPageButtonBase.Click += new RoutedEventHandler(this.OnFirstPageButtonBaseClick);
                AutomationProperties.SetAutomationId(this._firstPageButtonBase, DATAPAGER_firstPageButtonAutomationId);
            }

            if (this._previousPageButtonBase != null)
            {
                this._previousPageButtonBase.Click += new RoutedEventHandler(this.OnPreviousPageButtonBaseClick);
                AutomationProperties.SetAutomationId(this._previousPageButtonBase, DATAPAGER_previousPageButtonAutomationId);
            }

            if (this._nextPageButtonBase != null)
            {
                this._nextPageButtonBase.Click += new RoutedEventHandler(this.OnNextPageButtonBaseClick);
                AutomationProperties.SetAutomationId(this._nextPageButtonBase, DATAPAGER_nextPageButtonAutomationId);
            }

            if (this._lastPageButtonBase != null)
            {
                this._lastPageButtonBase.Click += new RoutedEventHandler(this.OnLastPageButtonBaseClick);
                AutomationProperties.SetAutomationId(this._lastPageButtonBase, DATAPAGER_lastPageButtonAutomationId);
            }

            // remove previous panel + buttons.
            if (this._numericButtonPanel != null)
            {
                this._numericButtonPanel.Children.Clear();
            }

            this._numericButtonPanel = GetTemplateChild(DATAPAGER_elementNumericButtonPanel) as Panel;

            // add new buttons to panel
            if (this._numericButtonPanel != null)
            {
                if (this._numericButtonPanel.Children.Count > 0)
                {
                    throw new InvalidOperationException(PagerResources.InvalidButtonPanelContent);
                }

                this.UpdateButtonCount();
            }

            this._currentPageTextBox = GetTemplateChild(DATAPAGER_elementCurrentPageTextBox) as TextBox;
            this._currentPagePrefixTextBlock = GetTemplateChild(DATAPAGER_elementCurrentPagePrefixTextBlockn) as TextBlock;
            this._currentPageSuffixTextBlock = GetTemplateChild(DATAPAGER_elementCurrentPageSuffixTextBlock) as TextBlock;

            if (this._currentPageTextBox != null)
            {
                this._currentPageTextBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.OnCurrentPageTextBoxKeyDown);
                this._currentPageTextBox.LostFocus += new RoutedEventHandler(this.OnCurrentPageTextBoxLostFocus);
                AutomationProperties.SetAutomationId(this._currentPageTextBox, DATAPAGER_currentPageTextBoxAutomationId);
            }

            this.UpdateControl();
        }

#endregion Public Methods

        ////------------------------------------------------------
        ////
        ////  Protected Methods
        ////
        ////------------------------------------------------------ 

#region Protected Methods

        /// <summary>
        /// Creates an AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>Automation Peer for this <see cref="T:System.Windows.Controls.DataPager" /> control</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataPagerAutomationPeer(this);
        }

#endregion Protected Methods

        ////------------------------------------------------------
        ////
        ////  Private Static Methods
        ////
        ////------------------------------------------------------ 

#region Private Static Methods

        /// <summary>
        /// AutoEllipsis property changed handler.
        /// </summary>
        /// <param name="d">NumericButton that changed its AutoEllipsis.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoEllipsisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            pager.UpdateButtonDisplay();
        }

        /// <summary>
        /// DisplayMode property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its DisplayMode.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            if (!pager.AreHandlersSuspended())
            {
                if (!Enum.IsDefined(typeof(PagerDisplayMode), e.NewValue))
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture,
                            Resource.InvalidEnumArgumentException_InvalidEnumArgument,
                            "value",
                            e.NewValue.ToString(),
                            typeof(PagerDisplayMode).Name));
                }

                pager.UpdateControl();
            }
        }

        /// <summary>
        /// IsTotalItemCountFixed property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed IsTotalItemCountFixed.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsTotalItemCountFixedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            pager.UpdateControl();
        }

        /// <summary>
        /// NumericButtonCount property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its NumericButtonCount.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnNumericButtonCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (!pager.AreHandlersSuspended())
            {
                if ((int)e.NewValue < 0)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            CultureInfo.InvariantCulture,
                            PagerResources.ValueMustBeGreaterThanOrEqualTo,
                            "NumericButtonCount",
                            0));
                }

                pager.UpdateButtonCount();
            }
        }

        /// <summary>
        /// NumericButtonStyle property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its NumericButtonStyle.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnNumericButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (pager._numericButtonPanel != null)
            {
                // update button styles
                foreach (UIElement uiElement in pager._numericButtonPanel.Children)
                {
                    ToggleButton button = uiElement as ToggleButton;
                    if (button != null)
                    {
                        button.Style = pager.NumericButtonStyle;
                    }
                }

                // update numeric toggle buttons foreground
                pager.UpdateNumericButtonsForeground();
            }
        }
        
        /// <summary>
        /// PageIndex property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its PageIndex.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnPageIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (!pager.AreHandlersSuspended())
            {
                int newPageIndex = (int)e.NewValue;

                if ((pager.Source == null || pager.PageSize == 0) && newPageIndex != -1)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        PagerResources.PageIndexMustBeNegativeOne);
                }

                if (pager.Source != null && pager.PageSize != 0 && newPageIndex < 0)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            CultureInfo.InvariantCulture,
                            PagerResources.ValueMustBeGreaterThanOrEqualTo,
                            "PageIndex",
                            0));
                }

                if (pager.PagedSource != null)
                {
                    if (newPageIndex != pager.PagedSource.PageIndex)
                    {
                        pager.PageMoveHandler((int) e.OldValue, newPageIndex, null);
                    }
                }
                else if (pager.Source != null)
                {
                    if (pager.PageSize != 0 && newPageIndex != 0)
                    {
                        // When the Source is an IEnumerable the PageIndex must be 0
                        pager.SetValueNoCallback(e.Property, e.OldValue);
                    }
                    else
                    {
                        // PageIndex changes are not cancellable when the Source is an IEnumerable.
                        pager.RaisePageIndexChangeEvents(true /*raisePageChanged*/);
                    }
                }
                else if (newPageIndex == -1)
                {
                    // Source is reset and PageIndex goes from >= 0 to -1
                    pager.RaisePageIndexChangeEvents(true /*raisePageChanged*/);
                }
                else
                {
                    // keep value set to -1 if there is no source
                    pager.SetValueNoCallback(e.Property, -1);
                }
            }
        }

        /// <summary>
        /// PageSize property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its PageSize.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnPageSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            if (!pager.AreHandlersSuspended())
            {
                int newPageSize = (int)e.NewValue;

                if (newPageSize < 0)
                {
                    pager.SetValueNoCallback(e.Property, e.OldValue);
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            CultureInfo.InvariantCulture,
                            PagerResources.ValueMustBeGreaterThanOrEqualTo,
                            "PageSize",
                            0));
                }

                if (pager.PagedSource != null)
                {
                    try
                    {
                        pager.PagedSource.PageSize = newPageSize;
                    }
                    catch
                    {
                        pager.SetValueNoCallback(e.Property, e.OldValue);
                        throw;
                    }
                }
                else if (pager.Source != null)
                {
                    pager.PageIndex = pager.PageSize == 0 ? -1 : 0;
                }

                pager.UpdateControl();
            }
        }

        /// <summary>
        /// PrivateForeground property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its Foreground property</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnPrivateForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            // Push the new foreground into the numeric toggle buttons
            pager.UpdateNumericButtonsForeground();
        }

        /// <summary>
        /// Called when a Read-Only dependency property is changed
        /// </summary>
        /// <param name="d">DataPager that changed its read-only property.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;
            if (pager != null && !pager.AreHandlersSuspended())
            {
                pager.SetValueNoCallback(e.Property, e.OldValue);
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        PagerResources.UnderlyingPropertyIsReadOnly,
                        e.Property.ToString()));
            }
        }

        /// <summary>
        /// SourceProperty property changed handler.
        /// </summary>
        /// <param name="d">DataPager that changed its Source.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager pager = d as DataPager;

            INotifyPropertyChanged oldNotifyPropertyChanged = e.OldValue as INotifyPropertyChanged;
            if (oldNotifyPropertyChanged != null && pager._weakEventListenerPropertyChanged != null)
            {
                pager._weakEventListenerPropertyChanged.Detach();
                pager._weakEventListenerPropertyChanged = null;
            }

            IPagedCollectionView newPagedCollectionView = e.NewValue as IPagedCollectionView;
            if (newPagedCollectionView != null)
            {
                INotifyPropertyChanged newNotifyPropertyChanged = e.NewValue as INotifyPropertyChanged;
                if (newNotifyPropertyChanged != null)
                {
                    pager._weakEventListenerPropertyChanged = new WeakEventListener<DataPager, object, PropertyChangedEventArgs>(pager);
                    pager._weakEventListenerPropertyChanged.OnEventAction = (instance, source, eventArgs) => instance.OnSourcePropertyChanged(source, eventArgs);
                    pager._weakEventListenerPropertyChanged.OnDetachAction = (weakEventListener) => newNotifyPropertyChanged.PropertyChanged -= weakEventListener.OnEvent;
                    newNotifyPropertyChanged.PropertyChanged += pager._weakEventListenerPropertyChanged.OnEvent;
                }

                if (pager.PageSize != 0)
                {
                    newPagedCollectionView.PageSize = pager.PageSize;
                }
                else
                {
                    pager.PageSize = newPagedCollectionView.PageSize;
                }
                if (pager.PageIndex != newPagedCollectionView.PageIndex)
                {
                    if (newPagedCollectionView.PageIndex == -1 && newPagedCollectionView.IsPageChanging)
                    {
                        // Avoid ArgumentOutOfRangeException in situation where the 
                        // IPagedCollectionView's PageIndex is still set to -1 while 
                        // a page move is in progress
                        pager.SetValueNoCallback(DataPager.PageIndexProperty, -1);
                    }
                    else
                    {
                        pager.PageIndex = newPagedCollectionView.PageIndex;
                    }
                    // Raise PageIndex change notifications for a non-cancellable change
                    pager.RaisePageIndexChangeEvents(true /*raisePageChanged*/);
                }
                pager.ItemCount = newPagedCollectionView.ItemCount;
                pager.UpdatePageCount();
                if (newPagedCollectionView.IsPageChanging)
                {
                    // Raise non-cancellable PageIndex changing notification since the source is already 
                    // in the middle of a page change
                    pager.RaisePageIndexChangeEvents(false /*raisePageChanged*/);
                }
            }
            else
            {
                IEnumerable enumerable = e.NewValue as IEnumerable;
                if (enumerable != null)
                {
                    IEnumerable<object> genericEnumerable = enumerable.Cast<object>();
                    pager.ItemCount = genericEnumerable.Count();
                    pager.PageCount = 1;
                    pager.PageIndex = pager.PageSize == 0 ? -1 : 0;
                }
                else
                {
                    pager.ItemCount = 0;
                    pager.PageCount = 0;
                    pager.PageIndex = -1;
                }
            }

            pager.UpdateControl();
        }

#endregion Private Static Methods

        ////------------------------------------------------------
        ////
        ////  Private Methods
        ////
        ////------------------------------------------------------ 

#region Private Methods

        /// <summary>
        /// Gets the starting index that our buttons should be labeled with.
        /// </summary>
        /// <returns>Starting index for our buttons</returns>
        private int GetButtonStartIndex()
        {
            // Because we have a starting PageIndex, we want to try and center the current pages
            // around this value. But if we are at the end of the collection, we display the last 
            // available buttons.
            return Math.Min(
                Math.Max((this.PageIndex + 1) - (this.NumericButtonCount / 2), 1), /* center buttons around pageIndex */
                Math.Max(this.PageCount - this.NumericButtonCount + 1, 1));        /* lastPage - number of buttons */
        }

        /// <summary>
        /// Attempts to move the current page index to the value
        /// in the current page textbox.
        /// </summary>
        private void MoveCurrentPageToTextboxValue()
        {
            if (this._currentPageTextBox.Text != (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture))
            {
                if (this.PagedSource != null && this.TryParseTextBoxPage())
                {
                    this.MoveToRequestedPage();
                }
                this._currentPageTextBox.Text = (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Given the new value of _requestedPageIndex, this method will attempt a page move 
        /// and set the _currentPageIndex variable accordingly.
        /// </summary>
        private void MoveToRequestedPage()
        {
            if (this._requestedPageIndex >= 0 && this._requestedPageIndex < this.PageCount)
            {
                // Requested page is within the known range
                this.PageIndex = this._requestedPageIndex;
            }
            else if (this._requestedPageIndex >= this.PageCount)
            {
                if (this.IsTotalItemCountFixed && this.PagedSource.TotalItemCount != -1)
                {
                    this.PageIndex = this.PageCount - 1;
                }
                else
                {
                    this.PageIndex = this._requestedPageIndex;
                }
            }
        }

        /// <summary>
        /// Handles the KeyDown event on the current page text box.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnCurrentPageTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.MoveCurrentPageToTextboxValue();
            }
        }

        /// <summary>
        /// Handles the loss of focus for the current page text box.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnCurrentPageTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.MoveCurrentPageToTextboxValue();
        }

        /// <summary>
        /// Handles the notifications for the DataPager.IsEnabled changes
        /// </summary>
        /// <param name="sender">DataPager that changed its IsEnabled property</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private void OnDataPagerIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCommonState();
        }

        /// <summary>
        /// Handles the click of the first page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnFirstPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.PagedSource != null)
            {
                int oldPageIndex = this.PagedSource.PageIndex;
                if (oldPageIndex != 0)
                {
                    this.PageMoveHandler(oldPageIndex, -1, this.PagedSource.MoveToFirstPage);
                }
            }
        }

        /// <summary>
        /// Handles the click of the last page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnLastPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.PagedSource != null)
            {
                int oldPageIndex = this.PagedSource.PageIndex;
                if (oldPageIndex != this.PageCount)
                {
                    this.PageMoveHandler(oldPageIndex, -1, this.PagedSource.MoveToLastPage);
                }
            }
        }

        /// <summary>
        /// Handles the click of the next page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnNextPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.PagedSource != null)
            {
                int oldPageIndex = this.PagedSource.PageIndex;
                if (oldPageIndex != this.PageIndex + 1)
                {
                    this.PageMoveHandler(oldPageIndex, -1, this.PagedSource.MoveToNextPage);
                }
            }
        }

        /// <summary>
        /// Handles the click of the previous page ButtonBase.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnPreviousPageButtonBaseClick(object sender, RoutedEventArgs e)
        {
            if (this.PagedSource != null)
            {
                int oldPageIndex = this.PagedSource.PageIndex;
                if (oldPageIndex != this.PageIndex - 1)
                {
                    this.PageMoveHandler(oldPageIndex, -1, this.PagedSource.MoveToPreviousPage);
                }
            }
        }

        /// <summary>
        /// This helper method will take care of calling the specified page move
        /// operation on the source collection, or MoveToPage if left null, while 
        /// also firing the PageIndexChanging and PageIndexChanged events.
        /// </summary>
        /// <param name="oldPageIndex">The oldPageIndex value before we change pages</param>
        /// <param name="newPageIndex">The page index to use with MoveToPage. This argument is ignored otherwise</param>
        /// <param name="pageMoveOperation">The delegate to call, or null when MoveToPage must be called</param>
        private void PageMoveHandler(int oldPageIndex, int newPageIndex, PageMoveOperationDelegate pageMoveOperation)
        {
            CancelEventArgs cancelArgs = new CancelEventArgs(false);
            this.RaisePageIndexChanging(cancelArgs);

            // When the IPagedCollectionView implementation updates its PageIndex property,
            // the DataPager gets a notification and raises the PageIndexChanged event.
            if (cancelArgs.Cancel)
            {
                // Revert back to old value, since operation was canceled
                this.SetValueNoCallback(DataPager.PageIndexProperty, oldPageIndex);
            }
            else
            {
                bool pageMoveOperationResult;
                if (pageMoveOperation == null)
                {
                    Debug.Assert(this.PagedSource != null, "Unexpected this.PagedSource == null");
                    pageMoveOperationResult = this.PagedSource.MoveToPage(newPageIndex);
                }
                else
                {
                    pageMoveOperationResult = pageMoveOperation();
                }
                if (!pageMoveOperationResult)
                {
                    // Revert back to old value, since operation failed
                    this.SetValueNoCallback(DataPager.PageIndexProperty, oldPageIndex);
                    // The PageIndexChanged needs to be raised even though no move occurred, 
                    // because of the PageIndexChanging notification above.
                    this.RaisePageIndexChanged();
                }
            }
        }

        /// <summary>
        /// Handles a property change within the Source.
        /// </summary>
        /// <param name="sender">The object firing this event.</param>
        /// <param name="e">The event args for this event.</param>
        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Debug.Assert(this.PagedSource != null, "Unexpected null this.PagedSource");

            switch (e.PropertyName)
            {
                case "Count":
                case "ItemCount":
                    this.ItemCount = this.PagedSource.ItemCount;
                    this.UpdatePageCount();
                    this.UpdateControl();
                    break;

                case "PageIndex":
                    int oldPageIndex = this.PageIndex;

                    this.PageIndex = this.PagedSource.PageIndex;
                    this.RaisePageIndexChanged();

                    DataPagerAutomationPeer peer = DataPagerAutomationPeer.FromElement(this) as DataPagerAutomationPeer;
                    if (peer != null && oldPageIndex != this.PageIndex)
                    {
                        peer.RefreshPageIndex(oldPageIndex);
                    }
                    break;

                case "PageSize":
                    this.PageSize = this.PagedSource.PageSize;
                    this.UpdatePageCount();
                    this.UpdateControl();
                    break;

                case "CanChangePage":
                case "Filter":
                case "TotalItemCount":
                case "SortDescriptions":
                    this.UpdateControl();
                    break;
            }
        }

        /// <summary>
        /// Raises a non-cancellable PageIndexChanging and optional PageIndexChanged events.
        /// </summary>
        /// <param name="raisePageChanged">True when the PageChanged event needs to be raised</param>
        private void RaisePageIndexChangeEvents(bool raisePageChanged)
        {
            this.RaisePageIndexChanging(new CancelEventArgs(false));
            if (raisePageChanged)
            {
                this.RaisePageIndexChanged();
            }
        }

        /// <summary>
        /// Raises the PageIndexChanged event.
        /// </summary>
        private void RaisePageIndexChanged()
        {
            this.UpdateControl();

            if (this._needPageChangingNotification)
            {
                this.RaisePageIndexChangeEvents(false /*raisePageChanged*/);
            }
            this._needPageChangingNotification = true;

            EventHandler<EventArgs> handler = this.PageIndexChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the PageIndexChanging event.
        /// </summary>
        /// <param name="e">The event args to use for the event.</param>
        private void RaisePageIndexChanging(CancelEventArgs e)
        {
            if (this._needPageChangingNotification)
            {
                EventHandler<CancelEventArgs> handler = this.PageIndexChanging;
                if (handler != null)
                {
                    handler(this, e);
                }

                // A PageIndexChanging notification is still required when 
                // the change was cancelled.
                if (!e.Cancel)
                {
                    this._needPageChangingNotification = false;
                }
            }
        }

        /// <summary>
        /// Update DataPager UI for paging enabled.
        /// </summary>
        /// <param name="needPage">Boolean that specifies if a page is needed</param>
        private void SetCannotChangePage(bool needPage)
        {
            if (this._currentPageTextBox != null && !needPage)
            {
                this._currentPageTextBox.Text = String.Empty;
            }

            VisualStateManager.GoToState(this, DATAPAGER_stateMoveDisabled, true);
            VisualStateManager.GoToState(this, DATAPAGER_stateMoveFirstDisabled, true);
            VisualStateManager.GoToState(this, DATAPAGER_stateMovePreviousDisabled, true);
            VisualStateManager.GoToState(this, DATAPAGER_stateMoveNextDisabled, true);
            VisualStateManager.GoToState(this, DATAPAGER_stateMoveLastDisabled, true);
        }

        /// <summary>
        /// Update DataPager UI for paging disabled.
        /// </summary>
        private void SetCanChangePage()
        {
            VisualStateManager.GoToState(this, DATAPAGER_stateMoveEnabled, true);

            if (this._currentPageTextBox != null)
            {
                this._currentPageTextBox.Text = (this.PageIndex + 1).ToString(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Notification raised when a numeric toggle button gets checked
        /// </summary>
        /// <param name="sender">The numeric toggle button</param>
        /// <param name="e">Routed event for the notification</param>
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this._ignoreToggleButtonCheckedNotification)
            {
                return;
            }

            // Ignore notifications when the source is an IEnumerable
            if (this.PagedSource != null)
            {
                ToggleButton button = sender as ToggleButton;
                int uiIndex = this._numericButtonPanel.Children.IndexOf(button);
                int pageIndex = this.GetButtonStartIndex() + uiIndex - 1;
                
                this.PageMoveHandler(this.PageIndex, pageIndex, null);

                if (this.PagedSource.PageIndex != pageIndex)
                {
                    try
                    {
                        this._ignoreToggleButtonUncheckedNotification = true;
                        // The toggle button that was checked must remain unchecked
                        // while the page move occurs, or because the page move initiation failed.
                        button.IsChecked = false;
                    }
                    finally
                    {
                        this._ignoreToggleButtonUncheckedNotification = false;
                    }
                }
            }
        }

        /// <summary>
        /// Notification raised when a numeric toggle button gets focus
        /// </summary>
        /// <param name="sender">The numeric toggle button</param>
        /// <param name="e">Routed event for the notification</param>
        private void ToggleButton_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!this._ignoreToggleButtonFocusNotification)
            {
                ToggleButton button = sender as ToggleButton;
                int uiIndex = this._numericButtonPanel.Children.IndexOf(button);
                // Remember which toggle button got focus so the same page index can 
                // regain focus when the numeric buttons are shifted.
                this._focusedToggleButtonIndex = this.GetButtonStartIndex() + uiIndex - 1;
            }
        }

        /// <summary>
        /// Notification raised when a numeric toggle button loses focus
        /// </summary>
        /// <param name="sender">The numeric toggle button</param>
        /// <param name="e">Routed event for the notification</param>
        private void ToggleButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this._ignoreToggleButtonFocusNotification)
            {
                // -1 is an indication that no toggle button has focus
                this._focusedToggleButtonIndex = -1;
            }
        }

        /// <summary>
        /// Notification raised when a numeric toggle button gets unchecked
        /// </summary>
        /// <param name="sender">The numeric toggle button</param>
        /// <param name="e">Routed event for the notification</param>
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this._ignoreToggleButtonUncheckedNotification)
            {
                try
                {
                    this._ignoreToggleButtonCheckedNotification = true;
                    // Attempts to uncheck a numeric toggle button, other than willingly 
                    // by internal logic, must fail.
                    ToggleButton button = sender as ToggleButton;
                    button.IsChecked = true;
                }
                finally
                {
                    this._ignoreToggleButtonCheckedNotification = false;
                }
            }
        }

        /// <summary>
        /// Attempts to put the integer value of the string in _currentPageTextBox into _requestedPageIndex.
        /// </summary>
        /// <returns>Whether or not the parsing of the string succeeded.</returns>
        private bool TryParseTextBoxPage()
        {
            // 

            bool successfullyParsed = int.TryParse(this._currentPageTextBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out this._requestedPageIndex);

            if (successfullyParsed)
            {
                // Subtract one to make it zero-based.
                this._requestedPageIndex--;
            }

            return successfullyParsed;
        }

        /// <summary>
        /// Updates the visual display of the number of buttons that we display.
        /// </summary>
        private void UpdateButtonCount()
        {
            // what we should use as the button count
            int buttonCount = Math.Min(this.NumericButtonCount, this.PageCount);

            if (this._numericButtonPanel != null)
            {
                // add new
                while (this._numericButtonPanel.Children.Count < buttonCount)
                {
                    ToggleButton button = new ToggleButton();
                    button.Style = this.NumericButtonStyle;
                    button.Checked += new RoutedEventHandler(this.ToggleButton_Checked);
                    button.Unchecked += new RoutedEventHandler(this.ToggleButton_Unchecked);
                    button.GotFocus += new RoutedEventHandler(this.ToggleButton_GotFocus);
                    button.LostFocus += new RoutedEventHandler(this.ToggleButton_LostFocus);
                    this._numericButtonPanel.Children.Add(button);
                }

                // remove excess
                while (this._numericButtonPanel.Children.Count > buttonCount)
                {
                    ToggleButton button = this._numericButtonPanel.Children[0] as ToggleButton;
                    if (button != null)
                    {
                        button.Checked -= new RoutedEventHandler(this.ToggleButton_Checked);
                        button.Unchecked -= new RoutedEventHandler(this.ToggleButton_Unchecked);
                        button.GotFocus -= new RoutedEventHandler(this.ToggleButton_GotFocus);
                        button.LostFocus -= new RoutedEventHandler(this.ToggleButton_LostFocus);
                        this._numericButtonPanel.Children.Remove(button);
                    }
                }

                this.UpdateNumericButtonsForeground();
                this.UpdateButtonDisplay();
            }
        }

        /// <summary>
        /// Updates the visual content and style of the buttons that we display.
        /// </summary>
        private void UpdateButtonDisplay()
        {
            if (this._numericButtonPanel != null)
            {
                // what we should use as the start index
                int startIndex = this.GetButtonStartIndex();

                // what we should use as the button count
                int buttonCount = Math.Min(this.NumericButtonCount, this.PageCount);

                // by default no focus restoration needs to occur
                bool isToggleButtonFocused = false;

                int index = startIndex;
                foreach (UIElement ui in this._numericButtonPanel.Children)
                {
                    ToggleButton button = ui as ToggleButton;
                    if (button != null)
                    {
                        if (this.PagedSource == null)
                        {
                            Debug.Assert(index == 1, "Unexpected index value for IEnumerable Source");
                            // The single toggle button needs to be checked.
                            button.IsChecked = true;
                        }
                        else if (this.PagedSource != null && this.PagedSource.PageIndex == index - 1)
                        {
                            try
                            {
                                this._ignoreToggleButtonCheckedNotification = true;
                                // The toggle button corresponding to the Source's current page
                                // needs to be checked.
                                button.IsChecked = true;
                            }
                            finally
                            {
                                this._ignoreToggleButtonCheckedNotification = false;
                            }
                        }
                        else
                        {
                            if ((bool) button.IsChecked)
                            {
                                try
                                {
                                    this._ignoreToggleButtonUncheckedNotification = true;
                                    // All other toggle buttons needs to be unchecked.
                                    button.IsChecked = false;
                                }
                                finally
                                {
                                    this._ignoreToggleButtonUncheckedNotification = false;
                                }
                            }
                        }

                        if (this.AutoEllipsis && index == startIndex + buttonCount - 1 &&
                            (index != this.PageCount))
                        {
                            button.Content = PagerResources.AutoEllipsisString;
                        }
                        else
                        {
                            button.Content = index;
                        }

                        if (this._focusedToggleButtonIndex != -1 &&
                            this._focusedToggleButtonIndex == index - 1)
                        {
                            try
                            {
                                this._ignoreToggleButtonFocusNotification = true;
                                // When the numeric toggle buttons are shifted because the 
                                // checked one is centered, the previously focused button 
                                // needs to be shifted as well.
                                button.Focus();
                            }
                            finally
                            {
                                this._ignoreToggleButtonFocusNotification = false;
                            }
                            isToggleButtonFocused = true;
                        }

                        AutomationProperties.SetAutomationId(button, DATAPAGER_numericalButtonAutomationId + index.ToString(CultureInfo.CurrentCulture));

                        index++;
                    }
                }

                if (this._focusedToggleButtonIndex != -1 && !isToggleButtonFocused)
                {
                    // The page index of the previously focused toggle button is now out of range.
                    // Focus the toggle button representing the current page instead.
                    foreach (UIElement ui in this._numericButtonPanel.Children)
                    {
                        ToggleButton button = ui as ToggleButton;
                        if (button != null && (bool)button.IsChecked)
                        {
                            button.Focus();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the state related to the IsEnabled property
        /// </summary>
        private void UpdateCommonState()
        {
            VisualStateManager.GoToState(this, this.IsEnabled ? DATAPAGER_stateNormal : DATAPAGER_stateDisabled, true);
        }

        /// <summary>
        /// Updates the current page, the total pages, and the
        /// state of the control.
        /// </summary>
        private void UpdateControl()
        {
            this.UpdatePageModeDisplay();
            this.UpdateButtonCount();

            bool needPage = this.Source != null && 
                ((this.PagedSource == null && this.PageSize > 0) || 
                 (this.PagedSource != null && this.PagedSource.PageSize > 0));

            this.CanMoveToFirstPage = needPage && this.PageIndex > 0;

            this.CanMoveToPreviousPage = this.CanMoveToFirstPage;

            this.CanMoveToNextPage = needPage && this.PagedSource != null &&
                (!this.IsTotalItemCountFixed || this.PagedSource.TotalItemCount == -1 || this.PageIndex < this.PageCount - 1);

            this.CanMoveToLastPage = needPage && this.PagedSource != null && 
                this.PagedSource.TotalItemCount != -1 && this.PageIndex < this.PageCount - 1;

            this.CanChangePage = needPage && (this.PagedSource == null || this.PagedSource.CanChangePage);

            this.UpdateCurrentPagePrefixAndSuffix(needPage);

            if (!needPage || !this.CanChangePage)
            {
                this.SetCannotChangePage(needPage);
            }
            else
            {
                this.SetCanChangePage();
                this.UpdateCanPageFirstAndPrevious();
                this.UpdateCanPageNextAndLast();
            }

            DataPagerAutomationPeer peer = DataPagerAutomationPeer.FromElement(this) as DataPagerAutomationPeer;
            if (peer != null)
            {
                peer.RefreshProperties();
            }
        }

        /// <summary>
        /// Updates the states of whether the pager can page to the first
        /// and to the previous page.
        /// </summary>
        private void UpdateCanPageFirstAndPrevious()
        {
            VisualStateManager.GoToState(this, this.CanMoveToFirstPage ? DATAPAGER_stateMoveFirstEnabled : DATAPAGER_stateMoveFirstDisabled, true);
            VisualStateManager.GoToState(this, this.CanMoveToPreviousPage ? DATAPAGER_stateMovePreviousEnabled : DATAPAGER_stateMovePreviousDisabled, true);
        }

        /// <summary>
        /// Updates the states of whether the pager can page to the next
        /// and to the last page.
        /// </summary>
        private void UpdateCanPageNextAndLast()
        {
            VisualStateManager.GoToState(this, this.CanMoveToNextPage ? DATAPAGER_stateMoveNextEnabled : DATAPAGER_stateMoveNextDisabled, true);
            VisualStateManager.GoToState(this, this.CanMoveToLastPage ? DATAPAGER_stateMoveLastEnabled : DATAPAGER_stateMoveLastDisabled, true);
        }

        /// <summary>
        /// Goes into the TotalPageCountKnown or TotalPageCountUnknown state according to Source.TotalItemCount
        /// and updates the captions of the text blocks surrounding the current page text box.
        /// </summary>
        /// <param name="needPage">True when a Source is set and PageSize > 0</param>
        private void UpdateCurrentPagePrefixAndSuffix(bool needPage)
        {
            bool needTotalPageCountUnknownState = !needPage || (this.PagedSource != null && this.PagedSource.TotalItemCount == -1);
            string textBlockCaption;

            if (this._currentPagePrefixTextBlock != null)
            {
                if (this._currentPagePrefixTextBlock != null)
                {
                    if (needTotalPageCountUnknownState)
                    {
                        textBlockCaption = PagerResources.CurrentPagePrefix_TotalPageCountUnknown;
                    }
                    else
                    {
                        textBlockCaption = string.Format(
                            CultureInfo.InvariantCulture,
                            PagerResources.CurrentPagePrefix_TotalPageCountKnown,
                            this.PageCount.ToString(CultureInfo.CurrentCulture));
                    }

                    this._currentPagePrefixTextBlock.Text = textBlockCaption;
                    if (string.IsNullOrEmpty(textBlockCaption))
                    {
                        this._currentPagePrefixTextBlock.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this._currentPagePrefixTextBlock.Visibility = Visibility.Visible;
                    }
                }
            }

            if (this._currentPageSuffixTextBlock != null)
            {
                if (needTotalPageCountUnknownState)
                {
                    textBlockCaption = PagerResources.CurrentPageSuffix_TotalPageCountUnknown;
                }
                else
                {
                    textBlockCaption = string.Format(
                        CultureInfo.InvariantCulture,
                        PagerResources.CurrentPageSuffix_TotalPageCountKnown,
                        this.PageCount.ToString(CultureInfo.CurrentCulture));
                }

                this._currentPageSuffixTextBlock.Text = textBlockCaption;
                if (string.IsNullOrEmpty(textBlockCaption))
                {
                    this._currentPageSuffixTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this._currentPageSuffixTextBlock.Visibility = Visibility.Visible;
                }
            }

            VisualStateManager.GoToState(this, needTotalPageCountUnknownState ? DATAPAGER_stateTotalPageCountUnknown : DATAPAGER_stateTotalPageCountKnown, true);
        }

        /// <summary>
        /// Pushes this DataPager's Foreground into the numeric buttons when that property isn't set
        /// in their style.
        /// </summary>
        private void UpdateNumericButtonsForeground()
        {
            if (this._numericButtonPanel != null)
            {
                foreach (UIElement uiElement in this._numericButtonPanel.Children)
                {
                    ToggleButton button = uiElement as ToggleButton;
                    if (button != null)
                    {
                        bool useDataPagerForeground = true;
                        if (button.Style != null)
                        {
                            foreach (SetterBase sb in button.Style.Setters)
                            {
                                Setter s = sb as Setter;
                                if (s != null && s.Property != null &&
                                    s.Property.Equals(ToggleButton.ForegroundProperty) &&
                                    s.Value != null)
                                {
                                    // The toggle button's style explicitly sets the foreground,
                                    // let's not default to the DataPager's setting then.
                                    useDataPagerForeground = false;
                                    break;
                                }
                            }
                        }
                        if (useDataPagerForeground)
                        {
                            button.Foreground = this.Foreground;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the visual display to show the current page mode
        /// we have selected.
        /// </summary>
        private void UpdatePageModeDisplay()
        {
            VisualStateManager.GoToState(this, Enum.GetName(typeof(PagerDisplayMode), this.DisplayMode), true);
        }

        /// <summary>
        /// Updates the page count based on the number of items and the page size.
        /// </summary>
        private void UpdatePageCount()
        {
            if (this.PagedSource.PageSize > 0)
            {
                this.PageCount = Math.Max(1, (int)Math.Ceiling((double)this.PagedSource.ItemCount / this.PagedSource.PageSize));
            }
            else
            {
                this.PageCount = 1;
            }
        }

#endregion Private Methods
    }
}
