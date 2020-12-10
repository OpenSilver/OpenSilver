// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

#if MIGRATION
using System.Windows.Controls.Common;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.System;
using Windows.UI.Xaml.Controls.Common;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if WORKINPROGRESS && OPENSILVER
#if MIGRATION
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Automation.Peers;
#endif
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Displays a summary of validation errors on a form.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = PART_SummaryListBox, Type = typeof(ListBox))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = "Empty", GroupName = VisualStates.GroupValidation)]
    [TemplateVisualState(Name = "HasErrors", GroupName = VisualStates.GroupValidation)]
    [StyleTypedProperty(Property = "SummaryListBoxStyle", StyleTargetType = typeof(ListBox))]
    [StyleTypedProperty(Property = "ErrorStyle", StyleTargetType = typeof(ListBoxItem))]
    public class ValidationSummary : Control
    {
#region Static Fields and Constants

        private const string PART_SummaryListBox = "SummaryListBox";
        private const string PART_HeaderContentControl = "HeaderContentControl";

#endregion Static Fields and Constants

#region Member Fields

        private ValidationSummaryItemSource _currentValidationSummaryItemSource;
        private ValidationItemCollection _displayedErrors;
        private ValidationItemCollection _errors;
        private ListBox _errorsListBox;
        private ContentControl _headerContentControl;
        private bool _initialized;
        private FrameworkElement _registeredParent;
        private Dictionary<string, ValidationSummaryItem> _validationSummaryItemDictionary;

#endregion Member Fields

#region Events

        /// <summary>
        /// Event triggered when an Error is clicked on.
        /// </summary>
        public event EventHandler<FocusingInvalidControlEventArgs> FocusingInvalidControl;

        /// <summary>
        /// Event triggered when the selected error has changed.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

#endregion Events

#region	Constructors

        /// <summary>
        /// Initializes a new instance of the ValidationSummary class.
        /// </summary>
        public ValidationSummary()
        {
            this.DefaultStyleKey = typeof(ValidationSummary);
            this._errors = new ValidationItemCollection();
            this._validationSummaryItemDictionary = new Dictionary<string, ValidationSummaryItem>();
            this._displayedErrors = new ValidationItemCollection();
            this._errors.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Errors_CollectionChanged);
            this.Loaded += new RoutedEventHandler(this.ValidationSummary_Loaded);
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.ValidationSummary_IsEnabledChanged);
            if (DesignerProperties.IsInDesignTool)
            {
                this.Errors.Add(new ValidationSummaryItem("Sample Error", typeof(ValidationSummaryItem).Name, ValidationSummaryItemType.ObjectError, null, null));
                this.Errors.Add(new ValidationSummaryItem("Sample Error", typeof(ValidationSummaryItem).Name, ValidationSummaryItemType.ObjectError, null, null));
                this.Errors.Add(new ValidationSummaryItem("Sample Error", typeof(ValidationSummaryItem).Name, ValidationSummaryItemType.ObjectError, null, null));
            }
        }

#endregion Constructors

#region Attached Properties

#region ShowErrorsInSummary

        /// <summary>
        /// Gets or sets a value indicating whether the field errors belonging to the input control should be shown in the ValidationSummary. 
        /// Errors are added to the DisplayedErrors list depending on this flag.  The base Errors list, however, will always contain all
        /// the errors.
        /// </summary>
        public static readonly DependencyProperty ShowErrorsInSummaryProperty = DependencyProperty.RegisterAttached(
            "ShowErrorsInSummary",
            typeof(bool),
            typeof(ValidationSummary),
            new PropertyMetadata(true, OnShowErrorsInSummaryPropertyChanged));

        /// <summary>
        /// Gets the ShowErrorsInSummary property of the specified DependencyObject.
        /// </summary>
        /// <param name="inputControl">The input control to get the ShowErrorsInSummary property from.</param>
        /// <returns>The value indicating whether errors on the input control should be shown.</returns>
        public static bool GetShowErrorsInSummary(DependencyObject inputControl)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            return (bool)inputControl.GetValue(ShowErrorsInSummaryProperty);
        }

        /// <summary>
        /// Sets the ShowErrorsInSummary property of the specified DependencyObject.
        /// </summary>
        /// <param name="inputControl">The input control with which to associate the specified dependency property.</param>
        /// <param name="value">The value indicating whether errors on the input control should be shown.</param>
        public static void SetShowErrorsInSummary(DependencyObject inputControl, bool value)
        {
            if (inputControl == null)
            {
                throw new ArgumentNullException("inputControl");
            }
            inputControl.SetValue(ShowErrorsInSummaryProperty, value);
        }

        private static void OnShowErrorsInSummaryPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement page = (Application.Current != null) ? Application.Current.RootVisual as FrameworkElement : null;
            if (page != null)
            {
                UpdateDisplayedErrorsOnAllValidationSummaries(page);
            }
        }

#endregion ShowErrorsInSummary

#endregion Attached Properties

#region Dependency Properties

#region ErrorStyle

        /// <summary>
        /// Identifies the ErrorStyle dependency property
        /// </summary>
        public static readonly DependencyProperty ErrorStyleProperty =
            DependencyProperty.Register(
            "ErrorStyle",
            typeof(Style),
            typeof(ValidationSummary),
            null);

        /// <summary>
        /// Gets or sets the style used for the error's item container.
        /// </summary>
        public Style ErrorStyle
        {
            get { return GetValue(ErrorStyleProperty) as Style; }
            set { SetValue(ErrorStyleProperty, value); }
        }

#endregion ErrorStyle

#region Filter

        /// <summary>
        /// Identifies the Filter dependency property
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(
            "Filter",
            typeof(ValidationSummaryFilters),
            typeof(ValidationSummary),
            new PropertyMetadata(ValidationSummaryFilters.All, OnFilterPropertyChanged));

        /// <summary>
        /// Gets or sets a value that indicates which types of errors are displayed.
        /// </summary>
        public ValidationSummaryFilters Filter
        {
            get { return (ValidationSummaryFilters)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        private static void OnFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationSummary vs = d as ValidationSummary;
            if (vs != null)
            {
                vs.UpdateDisplayedErrors();
            }
        }

#endregion Filter

#region FocusControlsOnClick

        /// <summary>
        /// Identifies the FocusControlsOnClick dependency property.
        /// </summary>
        public static readonly DependencyProperty FocusControlsOnClickProperty =
            DependencyProperty.Register(
            "FocusControlsOnClick",
            typeof(bool),
            typeof(ValidationSummary),
            new PropertyMetadata(true, null));

        /// <summary>
        /// Gets or sets a value that indicates whether focus is set on the 
        /// input control when an error message is clicked.
        /// </summary>
        public bool FocusControlsOnClick
        {
            get { return (bool)GetValue(FocusControlsOnClickProperty); }
            set { SetValue(FocusControlsOnClickProperty, value); }
        }

#endregion FocusControlsOnClick

#region HasErrors

        /// <summary>
        /// Identifies the HasErrors dependency property
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register(
            "HasErrors",
            typeof(bool),
            typeof(ValidationSummary),
            new PropertyMetadata(false, OnHasErrorsPropertyChanged));

        /// <summary>
        ///   Gets or sets a value that indicates whether the <see cref="ValidationSummary" /> has errors. 
        /// </summary>
        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            internal set { this.SetValueNoCallback(HasErrorsProperty, value); }
        }

        private static void OnHasErrorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationSummary vs = d as ValidationSummary;
            if (vs != null && !vs.AreHandlersSuspended())
            {
                vs.SetValueNoCallback(ValidationSummary.HasErrorsProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "{0} cannot be set because the underlying property is read only.", "HasErrors"));
            }
        }

#endregion HasErrors

#region HasDisplayedErrors

        /// <summary>
        /// Identifies the HasDisplayedErrors dependency property
        /// </summary>
        public static readonly DependencyProperty HasDisplayedErrorsProperty =
            DependencyProperty.Register(
            "HasDisplayedErrors",
            typeof(bool),
            typeof(ValidationSummary),
            new PropertyMetadata(false, OnHasDisplayedErrorsPropertyChanged));

        /// <summary>
        ///   Gets or sets a value that indicates whether the 
        ///   <see cref="ValidationSummary" /> has displayed errors. 
        /// </summary>
        public bool HasDisplayedErrors
        {
            get { return (bool)GetValue(HasDisplayedErrorsProperty); }
            internal set { this.SetValueNoCallback(HasDisplayedErrorsProperty, value); }
        }

        private static void OnHasDisplayedErrorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationSummary vs = d as ValidationSummary;
            if (vs != null && !vs.AreHandlersSuspended())
            {
                vs.SetValueNoCallback(ValidationSummary.HasDisplayedErrorsProperty, e.OldValue);
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "{0} cannot be set because the underlying property is read only.", "HasDisplayedErrors"));
            }
        }

#endregion HasDisplayedErrors

#region Header

        /// <summary>
        /// Identifies the Header dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(ValidationSummary),
            new PropertyMetadata(OnHasHeaderPropertyChanged));

        /// <summary>
        /// Gets or sets the content of the <see cref="ValidationSummary" /> header. 
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static void OnHasHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationSummary vs = d as ValidationSummary;
            if (vs != null)
            {
                vs.UpdateHeaderText();
            }
        }

#endregion Header

#region HeaderTemplate

        /// <summary>
        /// Identifies the HeaderTemplate dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(ValidationSummary),
            null);

        /// <summary>
        /// Gets or sets the template that is used to display the content of the header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

#endregion HeaderTemplate

#region SummaryListBoxStyle

        /// <summary>
        /// Identifies the SummaryListBoxStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty SummaryListBoxStyleProperty =
            DependencyProperty.Register(
            "SummaryListBoxStyle",
            typeof(Style),
            typeof(ValidationSummary),
            null);

        /// <summary>
        /// Gets or sets the style applied to the <see cref="ListBox" /> that displays the errors. 
        /// </summary>
        public Style SummaryListBoxStyle
        {
            get { return GetValue(SummaryListBoxStyleProperty) as Style; }
            set { SetValue(SummaryListBoxStyleProperty, value); }
        }

#endregion SummaryListBoxStyle

#region Target

        /// <summary>
        /// Identifies the Target dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(
            "Target",
            typeof(UIElement),
            typeof(ValidationSummary),
            new PropertyMetadata(OnTargetPropertyChanged));

        /// <summary>
        /// Gets or sets the <see cref="UIElement" /> for 
        /// which validation errors will be displayed in the summary. 
        /// </summary>
        public UIElement Target
        {
            get { return GetValue(TargetProperty) as UIElement; }
            set { SetValue(TargetProperty, value); }
        }

        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement oldElement = e.OldValue as FrameworkElement;
            ValidationSummary vs = d as ValidationSummary;
            EventHandler<ValidationErrorEventArgs> handler = new EventHandler<ValidationErrorEventArgs>(vs.Target_BindingValidationError);
            if (vs._registeredParent != null)
            {
                vs._registeredParent.BindingValidationError -= handler;
                vs._registeredParent = null;
            }
            if (oldElement != null)
            {
                oldElement.BindingValidationError -= handler;
            }
            FrameworkElement newElement = e.NewValue as FrameworkElement;
            if (newElement != null)
            {
                newElement.BindingValidationError += handler;
            }

            // Clear the old property binding errors
            vs._errors.ClearErrors(ValidationSummaryItemType.PropertyError);
            vs.UpdateDisplayedErrors();
        }

#endregion Target

#endregion Dependency Properties

#region Properties

        /// <summary>
        /// Gets the collection of errors.
        /// </summary>
        public ObservableCollection<ValidationSummaryItem> Errors
        {
            get { return this._errors; }
        }

        /// <summary>
        /// Gets the collection of errors that are displayed after 
        /// the <see cref="ValidationSummary.Filter" /> is applied. 
        /// </summary>
        public ReadOnlyObservableCollection<ValidationSummaryItem> DisplayedErrors
        {
            get { return new ReadOnlyObservableCollection<ValidationSummaryItem>(this._displayedErrors); }
        }

        /// <summary>
        /// Gets a value indicating whether the ValidationSummary is initialized.
        /// </summary>
        internal bool Initialized
        {
            get { return this._initialized; }
        }

        /// <summary>
        /// Gets the ErrorsListBox template part
        /// </summary>
        internal ListBox ErrorsListBoxInternal
        {
            get { return this._errorsListBox; }
        }

        /// <summary>
        /// Gets the HeaderContentControl template part
        /// </summary>
        internal ContentControl HeaderContentControlInternal
        {
            get { return this._headerContentControl; }
        }

#endregion Properties

#region	Methods

#region Static Methods

        /// <summary>
        /// Compare ValidationSummaryItems for display in the ValidationSummary
        /// </summary>
        /// <param name="x">The first reference used for comparison.</param>
        /// <param name="y">The second reference used for comparison.</param>
        /// <returns>The result of the comparison check between the two references.</returns>
        internal static int CompareValidationSummaryItems(ValidationSummaryItem x, ValidationSummaryItem y)
        {
            int returnVal;
            if (!ReferencesAreValid(x, y, out returnVal))
            {
                // Do a null comparison check if one (or both) are null
                return returnVal;
            }

            // Compare ErrorSource
            if (TryCompareReferences(x.ItemType, y.ItemType, out returnVal))
            {
                return returnVal;
            }

            // Compare Control
            Control controlX = x.Sources.Count > 0 ? x.Sources[0].Control : null;
            Control controlY = y.Sources.Count > 0 ? y.Sources[0].Control : null;

            if (controlX != controlY)
            {
                if (!ReferencesAreValid(controlX, controlY, out returnVal))
                {
                    // Do a null comparison check if one is null
                    return returnVal;
                }

                // Both are controls
                if (controlX.TabIndex != controlY.TabIndex)
                {
                    // Sort by TabIndex
                    return controlX.TabIndex.CompareTo(controlY.TabIndex);
                }

                // Both share the same parent, sort by index
                returnVal = SortByVisualTreeOrdering(controlX, controlY);
                if (returnVal != 0)
                {
                    return returnVal;
                }

                // TabIndexes and ordering are the same, move to next check
                if (TryCompareReferences(controlX.Name, controlY.Name, out returnVal))
                {
                    return returnVal;
                }
            }

            // If we reached this point, we could not compare by Control, TabIndex, nor Name.  
            // Compare by MessageHeader
            if (TryCompareReferences(x.MessageHeader, y.MessageHeader, out returnVal))
            {
                return returnVal;
            }

            // Compare by ErrorMessage
            TryCompareReferences(x.Message, y.Message, out returnVal);
            return returnVal;
        }

        private static int FindMatchingErrorSource(IList<ValidationSummaryItemSource> sources, ValidationSummaryItemSource sourceToFind)
        {
            if (sources != null)
            {
                for (int i = 0; i < sources.Count; i++)
                {
                    if (sources[i].Equals(sourceToFind))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Try to compare two references, but only if they they are comparable
        /// </summary>
        /// <param name="x">The first reference to compare.</param>
        /// <param name="y">The second reference to compare.</param>
        /// <param name="returnVal">The comparison value.</param>
        /// <returns>Returns true if the two references were able to be compared.</returns>
        private static bool TryCompareReferences(object x, object y, out int returnVal)
        {
            // If the two references are equal, then they should not be used for comparison purposes (in this context)
            // and we should try to use the next set of candidate properties.
            if ((x == null && y == null) || (x != null && x.Equals(y)))
            {
                returnVal = 0;
                return false;
            }

            // Do a reference level comparison, such as if one field is null whereas the other is not null.
            if (!ReferencesAreValid(x, y, out returnVal))
            {
                return true;
            }

            // If both references are valid (non null), try a standard comparison.
            IComparable comparableX = x as IComparable;
            IComparable comparableY = y as IComparable;
            if (comparableX != null && comparableY != null)
            {
                returnVal = comparableX.CompareTo(comparableY);
                return true;
            }

            // Could not compare
            returnVal = 0;
            return false;
        }

        /// <summary>
        /// Perform a null comparison check if one (or both) are null
        /// </summary>
        /// <param name="x">The first reference to compare.</param>
        /// <param name="y">The second reference to compare.</param>
        /// <param name="val">The comparison value.</param>
        /// <returns>Returns true if both references are non-null</returns>
        private static bool ReferencesAreValid(object x, object y, out int val)
        {
            if (x == null)
            {
                val = (y == null) ? 0 : -1;
                return false;
            }
            else if (y == null)
            {
                val = 1;
                return false;
            }
            val = 0;
            return true;
        }

        /// <summary>
        /// When one of the input controls has its ShowErrorsInSummary property changed, we have to go through all the ValidationSummaries on the page and update them
        /// </summary>
        /// <param name="parent">The parent ValidationSummary</param>
        private static void UpdateDisplayedErrorsOnAllValidationSummaries(DependencyObject parent)
        {
            if (parent == null)
            {
                return;
            }

            ValidationSummary vs = parent as ValidationSummary;
            if (vs != null)
            {
                vs.UpdateDisplayedErrors();
                return;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                UpdateDisplayedErrorsOnAllValidationSummaries(child);
            }
        }

        #endregion Static Methods

        #region Public Methods

        /// <summary>
        /// When the template is applied, this loads all the template parts
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
#if MIGRATION
            MouseButtonEventHandler mouseUpHandler = new MouseButtonEventHandler(this.ErrorsListBox_MouseLeftButtonUp);
#else
            PointerEventHandler mouseUpHandler = new PointerEventHandler(this.ErrorsListBox_MouseLeftButtonUp);
#endif
            KeyEventHandler keyDownHandler = new KeyEventHandler(this.ErrorsListBox_KeyDown);
            SelectionChangedEventHandler selectionChangedHandler = new SelectionChangedEventHandler(this.ErrorsListBox_SelectionChanged);

            if (this._errorsListBox != null)
            {
                // If the ErrorsListBox was already set (due to multiple calls to OnApplyTemplate), unload the handlers first.
#if MIGRATION
                this._errorsListBox.MouseLeftButtonUp -= mouseUpHandler;
#else
                this._errorsListBox.PointerReleased -= mouseUpHandler;
#endif
                this._errorsListBox.KeyDown -= keyDownHandler;
                this._errorsListBox.SelectionChanged -= selectionChangedHandler;
            }

            this._errorsListBox = GetTemplateChild(PART_SummaryListBox) as ListBox;
            if (this._errorsListBox != null)
            {
#if MIGRATION
                this._errorsListBox.MouseLeftButtonUp += mouseUpHandler;
#else
                this._errorsListBox.PointerReleased += mouseUpHandler;
#endif
                this._errorsListBox.KeyDown += keyDownHandler;
                this._errorsListBox.ItemsSource = this.DisplayedErrors;
                this._errorsListBox.SelectionChanged += selectionChangedHandler;
            }
            this._headerContentControl = GetTemplateChild(PART_HeaderContentControl) as ContentControl;
            this.UpdateDisplayedErrors();
            this.UpdateCommonState(false);
            this.UpdateValidationState(false);
        }

        /// <summary>
        /// OnErrorClicked is invoked when an error in the ValidationSummary is clicked, via either the mouse or keyboard.
        /// </summary>
        /// <param name="e">The FocusingInvalidControlEventArgs for the event.</param>
        protected virtual void OnFocusingInvalidControl(FocusingInvalidControlEventArgs e)
        {
            EventHandler<FocusingInvalidControlEventArgs> handler = this.FocusingInvalidControl;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Public Methods

        #region Protected Methods

#if WORKINPROGRESS && OPENSILVER
        /// <summary>
        /// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
        /// </summary>
        /// <returns>The AutomationPeer associated with this ValidationSummary.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValidationSummaryAutomationPeer(this);
        }
#endif

#endregion Protected Methods

        #region Internal Methods

        /// <summary>
        /// Simulate a click
        /// </summary>
        internal void ExecuteClickInternal()
        {
            this.ExecuteClick(this.ErrorsListBoxInternal);
        }

        internal string GetHeaderString()
        {
            string errorString = this._displayedErrors.Count == 1 ? "1 Error" : String.Format(CultureInfo.InvariantCulture, "{0} Errors", this._displayedErrors.Count);
            return errorString;
        }

        /// <summary>
        /// Sort two dependency objects based on their relative positions within the VisualTree.  
        /// </summary>
        /// <param name="controlX">The first control being compared.</param>
        /// <param name="controlY">The second value being compared.</param>
        /// <returns>The sort value indicating the ordering between the two controls.</returns>
        internal static int SortByVisualTreeOrdering(DependencyObject controlX, DependencyObject controlY)
        {
            if (controlX == null || controlY == null || controlX == controlY)
            {
                return 0;
            }
            List<DependencyObject> parentChain = new List<DependencyObject>();
            DependencyObject node = controlX;
            parentChain.Add(node);
            while ((node = VisualTreeHelper.GetParent(node)) != null)
            {
                parentChain.Add(node);
            }

            node = controlY;
            DependencyObject lastNodeY = node;
            while ((node = VisualTreeHelper.GetParent(node)) != null)
            {
                int idxParentChain = parentChain.IndexOf(node);
                if (idxParentChain == 0)
                {
                    // X itself is the ancestor
                    return -1;
                }
                else if (idxParentChain > 0)
                {
                    // Common ancestor found
                    DependencyObject lastNodeX = parentChain[idxParentChain - 1];
                    if (lastNodeX == null || lastNodeY == null)
                    {
                        return 0;
                    }
                    int numChildren = VisualTreeHelper.GetChildrenCount(node);
                    for (int i = 0; i < numChildren; i++)
                    {
                        DependencyObject child = VisualTreeHelper.GetChild(node, i);
                        if (child == lastNodeY)
                        {
                            // Y appears first.  This is checked first, as it has precedence when xLastNode == yLastNode
                            return 1;
                        }
                        if (child == lastNodeX)
                        {
                            return -1;
                        }
                    }
                }
                lastNodeY = node;
            }
            return 0;
        }

#endregion Internal Methods

#region Private Methods

#if MIGRATION
        private void ErrorsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ExecuteClick(sender);
            }
        }
#else
        private void ErrorsListBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.ExecuteClick(sender);
            }
        }
#endif

#if MIGRATION
        private void ErrorsListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ExecuteClick(sender);
        }
#else
        private void ErrorsListBox_MouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
        {
            this.ExecuteClick(sender);
        }
#endif

        private void ErrorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventHandler<SelectionChangedEventArgs> handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Errors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ValidationSummaryItem vsi in e.OldItems)
                {
                    if (vsi != null)
                    {
                        vsi.PropertyChanged -= new PropertyChangedEventHandler(this.ValidationSummaryItem_PropertyChanged);
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (ValidationSummaryItem vsi in e.NewItems)
                {
                    if (vsi != null)
                    {
                        vsi.PropertyChanged += new PropertyChangedEventHandler(this.ValidationSummaryItem_PropertyChanged);
                    }
                }
            }
            this.HasErrors = this._errors.Count > 0;
            this.UpdateDisplayedErrors();
        }

        private void ExecuteClick(object sender)
        {
            ListBox lb = sender as ListBox;
            if (lb != null)
            {
                ValidationSummaryItem vsi = lb.SelectedItem as ValidationSummaryItem;
                if (vsi != null && this.FocusControlsOnClick)
                {
                    int idx;
                    // Ensure the currently selected item source is valid
                    if (vsi.Sources.Count == 0)
                    {
                        // Clear the current ESI source if the ESI has none, such as when the ESI has changed
                        this._currentValidationSummaryItemSource = null;
                    }
                    else
                    {
                        // If the current ESI source is not part of the current set, select the first one by default.
                        idx = FindMatchingErrorSource(vsi.Sources, this._currentValidationSummaryItemSource);
                        if (idx < 0)
                        {
                            this._currentValidationSummaryItemSource = vsi.Sources[0];
                        }
                    }

                    // Raise the event
                    FocusingInvalidControlEventArgs e = new FocusingInvalidControlEventArgs(vsi, this._currentValidationSummaryItemSource);
                    this.OnFocusingInvalidControl(e);

#if WORKINPROGRESS && OPENSILVER
                    // Raise the AutomationPeer event
                    ValidationSummaryAutomationPeer peer = ValidationSummaryAutomationPeer.FromElement(this) as ValidationSummaryAutomationPeer;
                    if (peer != null && AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
                    }
#endif

                    // Focus the target, which will usually be the current ESI source or the overwritten one
                    if (!e.Handled && e.Target != null && e.Target.Control != null)
                    {
                        e.Target.Control.Focus();
                    }

                    // Update currently selected item, but only if there are multiple ESI sources.  
                    if (vsi.Sources.Count > 0)
                    {
                        idx = FindMatchingErrorSource(vsi.Sources, e.Target);
                        idx = idx < 0 ? 0 : ++idx % vsi.Sources.Count;
                        this._currentValidationSummaryItemSource = vsi.Sources[idx];
                    }
                }
            }
        }

        private void Target_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            FrameworkElement inputControl = e.OriginalSource as FrameworkElement;
            if (e != null && e.Error != null && e.Error.ErrorContent != null && inputControl != null)
            {
                string message = e.Error.ErrorContent.ToString();
#if NETSTANDARD
                string key = String.IsNullOrEmpty(inputControl.Name) ? inputControl.GetHashCode().ToString(CultureInfo.InvariantCulture) : inputControl.Name;
#else
                string key = String.IsNullOrEmpty(inputControl.Name) ? inputControl.GetHashCode().ToString(null, CultureInfo.InvariantCulture) : inputControl.Name;
#endif
                key += message;
                if (this._validationSummaryItemDictionary.ContainsKey(key))
                {
                    ValidationSummaryItem existingError = this._validationSummaryItemDictionary[key];
                    this._errors.Remove(existingError);
                    this._validationSummaryItemDictionary.Remove(key);
                }
                if (e.Action == ValidationErrorEventAction.Added)
                {
                    if (GetShowErrorsInSummary(inputControl))
                    {
                        // New error
                        string propertyName = null;
                        object entity;
                        BindingExpression be;
                        ValidationMetadata vmd = ValidationHelper.ParseMetadata(inputControl, false, out entity, out be);
                        if (vmd != null)
                        {
                            propertyName = vmd.Caption;
                        }
                        ValidationSummaryItem vsi = new ValidationSummaryItem(message, propertyName, ValidationSummaryItemType.PropertyError, new ValidationSummaryItemSource(propertyName, inputControl as Control), null);
                        this._errors.Add(vsi);
                        this._validationSummaryItemDictionary[key] = vsi;
                    }
                }
            }
        }

        private void UpdateDisplayedErrors()
        {
            List<ValidationSummaryItem> newErrors = new List<ValidationSummaryItem>();
            Debug.Assert(this.Errors != null, "ValidationSummary.Errors should not be null");
            foreach (ValidationSummaryItem vsi in this.Errors)
            {
                if (vsi != null &&
                    ((vsi.ItemType == ValidationSummaryItemType.ObjectError && (this.Filter & ValidationSummaryFilters.ObjectErrors) != 0) ||
                    (vsi.ItemType == ValidationSummaryItemType.PropertyError && (this.Filter & ValidationSummaryFilters.PropertyErrors) != 0)))
                {
                    newErrors.Add(vsi);
                }
            }
            newErrors.Sort(CompareValidationSummaryItems);
            this._displayedErrors.Clear();
            foreach (ValidationSummaryItem vsi in newErrors)
            {
                this._displayedErrors.Add(vsi);
            }
            this.UpdateValidationState(true);
            this.UpdateHeaderText();
        }

        private void UpdateHeaderText()
        {
            if (this._headerContentControl != null)
            {
                if (this.Header != null)
                {
                    this._headerContentControl.Content = this.Header;
                }
                else
                {
                    this._headerContentControl.Content = this.GetHeaderString();
                }
            }
        }

        private void UpdateValidationState(bool useTransitions)
        {
            this.HasDisplayedErrors = this._displayedErrors.Count > 0;
            VisualStateManager.GoToState(this, this.HasDisplayedErrors ? "HasErrors" : "Empty", useTransitions);
        }

        private void UpdateCommonState(bool useTransitions)
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, VisualStates.StateNormal, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateDisabled, useTransitions);
            }
        }

        private void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Target == null && this._registeredParent == null)
            {
                this._registeredParent = VisualTreeHelper.GetParent(this) as FrameworkElement;
                if (this._registeredParent != null)
                {
                    this._registeredParent.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(this.Target_BindingValidationError);
                }
            }
            this.Loaded -= new RoutedEventHandler(this.ValidationSummary_Loaded);
            this._initialized = true;
        }

        private void ValidationSummary_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateCommonState(true);
        }

        private void ValidationSummaryItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ValidationSummaryItem.PROPERTYNAME_ITEMTYPE)
            {
                this.UpdateDisplayedErrors();
            }
        }

#endregion Private Methods

#endregion Methods
    }
}
