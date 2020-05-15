#if WORKINPROGRESS
#if MIGRATION
using System;
using System.Collections.ObjectModel;
using System.Windows.Automation.Peers;

namespace System.Windows.Controls
{
	//
	// Summary:
	//     Displays a summary of the validation errors on a form.
	[StyleTypedProperty(Property = "SummaryListBoxStyle", StyleTargetType = typeof(ListBox))]
	[StyleTypedProperty(Property = "ErrorStyle", StyleTargetType = typeof(ListBoxItem))]
	[TemplatePart(Name = "SummaryListBox", Type = typeof(ListBox))]
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "HasErrors", GroupName = "ValidationStates")]
	[TemplateVisualState(Name = "Empty", GroupName = "ValidationStates")]
	public partial class ValidationSummary : Control
	{
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.ErrorStyle dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.ErrorStyle dependency
		//     property.
		public static readonly DependencyProperty ErrorStyleProperty =
			DependencyProperty.Register("ErrorStyle",
										typeof(Style),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.Filter dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.Filter dependency
		//     property.
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter",
										typeof(ValidationSummaryFilters),
										typeof(ValidationSummary),
										new PropertyMetadata(ValidationSummaryFilters.All));
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.FocusControlsOnClick
		//     dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.FocusControlsOnClick
		//     dependency property.
		public static readonly DependencyProperty FocusControlsOnClickProperty =
			DependencyProperty.Register("FocusControlsOnClick",
										typeof(bool),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.HasDisplayedErrors dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.HasDisplayedErrors
		//     dependency property.
		public static readonly DependencyProperty HasDisplayedErrorsProperty =
			DependencyProperty.Register("HasDisplayedErrors",
										typeof(bool),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.HasErrors dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.HasErrors dependency
		//     property.
		public static readonly DependencyProperty HasErrorsProperty =
			DependencyProperty.Register("HasErrors",
										typeof(bool),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.Header dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.Header dependency
		//     property.
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header",
										typeof(object),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.HeaderTemplate dependency
		//     property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.HeaderTemplate
		//     dependency property.
		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate",
										typeof(DataTemplate),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.ShowErrorsInSummary
		//     attached property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.ShowErrorsInSummary
		//     attached property.
		public static readonly DependencyProperty ShowErrorsInSummaryProperty =
			DependencyProperty.RegisterAttached("ShowErrorsInSummary",
												typeof(bool),
												typeof(ValidationSummary),
												null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.SummaryListBoxStyle
		//     dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.SummaryListBoxStyle
		//     dependency property.
		public static readonly DependencyProperty SummaryListBoxStyleProperty =
			DependencyProperty.Register("SummaryListBoxStyle",
										typeof(Style),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Identifies the System.Windows.Controls.ValidationSummary.Target dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.ValidationSummary.Target dependency
		//     property.
		public static readonly DependencyProperty TargetProperty =
			DependencyProperty.Register("Target",
										typeof(UIElement),
										typeof(ValidationSummary),
										null);
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.ValidationSummary class.
		public ValidationSummary()
		{
		}

		//
		// Summary:
		//     Gets the collection of errors that are displayed after the System.Windows.Controls.ValidationSummary.Filter
		//     is applied.
		//
		// Returns:
		//     The collection of errors that are displayed after the System.Windows.Controls.ValidationSummary.Filter
		//     is applied.
		public ReadOnlyObservableCollection<ValidationSummaryItem> DisplayedErrors
		{
			get;
		}

		//
		// Summary:
		//     Gets the collection of errors.
		//
		// Returns:
		//     The collection of errors.
		public ObservableCollection<ValidationSummaryItem> Errors
		{
			get;
		}

		//
		// Summary:
		//     Gets or sets the style used for the error's item container.
		//
		// Returns:
		//     The style used for the error's item container.
		public Style ErrorStyle
		{
			get { return (Style)GetValue(ErrorStyleProperty); }
			set { SetValue(ErrorStyleProperty, value); }
		}

		//
		// Summary:
		//     Gets or sets a value that indicates which types of errors are displayed.
		//
		// Returns:
		//     One of the enumeration values that indicates which types of errors are displayed.
		//     The default is System.Windows.Controls.ValidationSummaryFilters.All.
		public ValidationSummaryFilters Filter
		{
			get { return (ValidationSummaryFilters)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		//
		// Summary:
		//     Gets or sets a value that indicates whether focus is set on the input control
		//     when an error message is clicked.
		//
		// Returns:
		//     true if focus is set on the input control when an error message is clicked; otherwise,
		//     false. The default is true.
		public bool FocusControlsOnClick
		{
			get { return (bool)GetValue(FocusControlsOnClickProperty); }
			set { SetValue(FocusControlsOnClickProperty, value); }
		}

		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Controls.ValidationSummary
		//     has displayed errors.
		//
		// Returns:
		//     true if the System.Windows.Controls.ValidationSummary has displayed errors; otherwise,
		//     false. The default is false.
		public bool HasDisplayedErrors
		{
			get { return (bool)GetValue(HasDisplayedErrorsProperty); }
		}

		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Controls.ValidationSummary
		//     has errors.
		//
		// Returns:
		//     true if the System.Windows.Controls.ValidationSummary has errors; otherwise,
		//     false. The default is false.
		public bool HasErrors
		{
			get { return (bool)GetValue(HasErrorsProperty); }
		}

		//
		// Summary:
		//     Gets or sets the content of the System.Windows.Controls.ValidationSummary header.
		//
		// Returns:
		//     The content of the System.Windows.Controls.ValidationSummary header. The default
		//     is null.
		public object Header
		{
			get { return (object)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		//
		// Summary:
		//     Gets or sets the template that is used to display the content of the header.
		//
		// Returns:
		//     The template that is used to display the content of the header.
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		//
		// Summary:
		//     Gets or sets the style applied to the System.Windows.Controls.ListBox that displays
		//     the errors.
		//
		// Returns:
		//     The style applied to the System.Windows.Controls.ListBox that displays the errors.
		//     The default is null.
		public Style SummaryListBoxStyle
		{
			get { return (Style)GetValue(SummaryListBoxStyleProperty); }
			set { SetValue(SummaryListBoxStyleProperty, value); }
		}

		//
		// Summary:
		//     Gets or sets the System.Windows.UIElement for which validation errors will be
		//     displayed in the summary.
		//
		// Returns:
		//     The System.Windows.UIElement for which validation errors will be displayed in
		//     the summary. The default is null.
		public UIElement Target
		{
			get { return (UIElement)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}

		//
		// Summary:
		//     Occurs when an error in the error summary list is clicked.
		public event EventHandler<FocusingInvalidControlEventArgs> FocusingInvalidControl;
		//
		// Summary:
		//     Occurs when the currently selected item in the error summary list changes.
		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
		//
		// Summary:
		//     Gets the value of the System.Windows.Controls.ValidationSummary.ShowErrorsInSummary
		//     attached property of the specified System.Windows.DependencyObject.
		//
		// Parameters:
		//   inputControl:
		//     The input control from which the property value is read.
		//
		// Returns:
		//     The System.Windows.Controls.ValidationSummary.ShowErrorsInSummary property value
		//     of the specified object.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     inputControl is null.
		public static bool GetShowErrorsInSummary(DependencyObject inputControl)
		{
			return false;
		}

		//
		// Summary:
		//     Sets the value of the System.Windows.Controls.ValidationSummary.ShowErrorsInSummary
		//     attached property of the specified System.Windows.DependencyObject.
		//
		// Parameters:
		//   inputControl:
		//     The input control with which to associate the attached property.
		//
		//   value:
		//     true if errors on the input control should be shown in the summary; otherwise,
		//     false.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     inputControl is null.
		public static void SetShowErrorsInSummary(DependencyObject inputControl, bool value)
		{
		}

		//
		// Summary:
		//     Builds the visual tree for the System.Windows.Controls.ValidationSummary when
		//     a new template is applied.
		public override void OnApplyTemplate()
		{
		}

#if no
        // Summary:
        //     Returns a System.Windows.Automation.Peers.ValidationSummaryAutomationPeer for
        //     use by the Silverlight automation infrastructure.
        //
        // Returns:
        //     A System.Windows.Automation.Peers.ValidationSummaryAutomationPeer for the System.Windows.Controls.ValidationSummary
        //     object.
        protected override AutomationPeer OnCreateAutomationPeer()
        {

        }
#endif
		//
		// Summary:
		//     Raises the System.Windows.Controls.ValidationSummary.FocusingInvalidControl event.
		//
		// Parameters:
		//   e:
		//     The event data.
		protected virtual void OnFocusingInvalidControl(FocusingInvalidControlEventArgs e)
		{
		}
	}
}
#endif
#endif