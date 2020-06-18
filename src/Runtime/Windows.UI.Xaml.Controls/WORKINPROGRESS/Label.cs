#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Automation.Peers;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class Label
	{
		//
		// Summary:
		//     Identifies the System.Windows.Controls.Label.IsRequired dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.Label.IsRequired dependency property.
		public static readonly DependencyProperty IsRequiredProperty =
			DependencyProperty.Register("IsRequired",
										typeof(bool),
										typeof(Label),
										new PropertyMetadata(false));
		//
		// Summary:
		//     Identifies the System.Windows.Controls.Label.IsValid dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.Label.IsValid dependency property.
		public static readonly DependencyProperty IsValidProperty =
			DependencyProperty.Register("IsValid",
										typeof(bool),
										typeof(Label),
										new PropertyMetadata(true));
		//
		// Summary:
		//     Identifies the System.Windows.Controls.Label.PropertyPath dependency property.
		//
		// Returns:
		//     The identifier for the System.Windows.Controls.Label.PropertyPath dependency
		//     property.
		public static readonly DependencyProperty PropertyPathProperty =
			DependencyProperty.Register("PropertyPath",
										typeof(string),
										typeof(Label),
										null);

		public static readonly DependencyProperty TargetProperty =
			DependencyProperty.Register("Target",
										typeof(FrameworkElement),
										typeof(Label),
										null);

		//
		// Summary:
		//     Gets or sets a value that indicates whether the property bound to the System.Windows.Controls.Label.Target
		//     field is required.
		//
		// Returns:
		//     true if the field is required; otherwise, false. The default is false.
		public bool IsRequired
		{
			get { return (bool)GetValue(IsRequiredProperty); }
			set { SetValue(IsRequiredProperty, value); }
		}
		//
		// Summary:
		//     Gets a value that indicates whether the System.Windows.Controls.Label.Target
		//     field data is valid.
		//
		// Returns:
		//     true if the field data is valid; otherwise, false. The default is true.
		public bool IsValid
		{
			get { return (bool)GetValue(IsValidProperty); }
		}
		//
		// Summary:
		//     Gets or sets the path to the dependency property on the System.Windows.FrameworkElement.DataContext
		//     of the System.Windows.Controls.Label.Target control that this System.Windows.Controls.Label
		//     is associated with.
		//
		// Returns:
		//     The path to the dependency property on the System.Windows.FrameworkElement.DataContext
		//     of the System.Windows.Controls.Label.Target control that this System.Windows.Controls.Label
		//     is associated with. The default is null.
		public string PropertyPath
		{
			get { return (string)GetValue(PropertyPathProperty); }
			set { SetValue(PropertyPathProperty, value); }
		}
		//
		// Summary:
		//     Gets or sets the control that this System.Windows.Controls.Label is associated
		//     with.
		//
		// Returns:
		//     The control that this System.Windows.Controls.Label is associated with.
		public FrameworkElement Target
		{
			get { return (FrameworkElement)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}

		//
		// Summary:
		//     Builds the visual tree for the System.Windows.Controls.Label control when a new
		//     template is applied.
#if MIGRATION
		public override void OnApplyTemplate()
#else
	    protected override void OnApplyTemplate()
#endif
		{

		}
		//
		// Summary:
		//     Reloads the metadata from the System.Windows.Controls.Label.Target element.
		public virtual void Refresh()
		{

		}
		//
		// Summary:
		//     Called when the value of the System.Windows.Controls.ContentControl.Content property
		//     changes.
		//
		// Parameters:
		//   oldContent:
		//     The old value of the System.Windows.Controls.ContentControl.Content property.
		//
		//   newContent:
		//     The new value of the System.Windows.Controls.ContentControl.Content property.
		protected override void OnContentChanged(object oldContent, object newContent)
		{

		}
		//
		// Summary:
		//     Returns a System.Windows.Automation.Peers.LabelAutomationPeer for use by the
		//     Silverlight automation infrastructure.
		//
		// Returns:
		//     A System.Windows.Automation.Peers.LabelAutomationPeer for the System.Windows.Controls.Label
		//     object.
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return default(AutomationPeer);
		}
	}
}

#endif