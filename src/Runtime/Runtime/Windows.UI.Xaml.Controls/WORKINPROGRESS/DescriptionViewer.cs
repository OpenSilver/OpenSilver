

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


//using System.Windows.Automation.Peers;
#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    //
    // Summary:
    //     Displays a description and tracks error state for an associated control.
    //[StyleTypedProperty(Property = "ToolTipStyle", StyleTargetType = typeof(ToolTip))]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "ValidUnfocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "NoDescription", GroupName = "DescriptionStates")]
    [TemplateVisualState(Name = "HasDescription", GroupName = "DescriptionStates")]
    [TemplateVisualState(Name = "ValidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    public class DescriptionViewer : Control
    {
        //
        // Summary:
        //     Identifies the System.Windows.Controls.DescriptionViewer.Description dependency
        //     property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.DescriptionViewer.Description
        //     dependency property.
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description",
                                        typeof(string),
                                        typeof(DescriptionViewer),
                                        null);
        //
        // Summary:
        //     Identifies the System.Windows.Controls.DescriptionViewer.GlyphTemplate dependency
        //     property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.DescriptionViewer.GlyphTemplate
        //     dependency property.
        public static readonly DependencyProperty GlyphTemplateProperty =
            DependencyProperty.Register("GlyphTemplate",
                                        typeof(ControlTemplate),
                                        typeof(DescriptionViewer),
                                        null);
        //
        // Summary:
        //     Identifies the System.Windows.Controls.DescriptionViewer.IsFocused dependency
        //     property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.DescriptionViewer.IsFocused dependency
        //     property.
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register("IsFocused",
                                        typeof(bool),
                                        typeof(DescriptionViewer),
                                        null);
        //
        // Summary:
        //     Identifies the System.Windows.Controls.DescriptionViewer.IsValid dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.DescriptionViewer.IsValid dependency
        //     property.
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid",
                                        typeof(bool),
                                        typeof(DescriptionViewer),
                                        null);
        //
        // Summary:
        //     Identifies the System.Windows.Controls.DescriptionViewer.PropertyPath dependency
        //     property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.DescriptionViewer.PropertyPath
        //     dependency property.
        public static readonly DependencyProperty PropertyPathProperty =
            DependencyProperty.Register("PropertyPath",
                                        typeof(string),
                                        typeof(DescriptionViewer),
                                        null);
        //
        // Summary:
        //     Identifies the System.Windows.Controls.DescriptionViewer.Target dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.DescriptionViewer.Target dependency
        //     property.
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target",
                                        typeof(FrameworkElement),
                                        typeof(DescriptionViewer),
                                        null);
        //
        // Summary:
        //     Identifies the System.Windows.Controls.DescriptionViewer.ToolTipStyle dependency
        //     property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.DescriptionViewer.ToolTipStyle
        //     dependency property.
        public static readonly DependencyProperty ToolTipStyleProperty =
            DependencyProperty.Register("ToolTipStyle",
                                        typeof(Style),
                                        typeof(DescriptionViewer),
                                        null);

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Controls.DescriptionViewer class.
        public DescriptionViewer()
        {

        }

        //
        // Summary:
        //     Gets or sets the description text displayed by the viewer.
        //
        // Returns:
        //     The description text displayed by the viewer.
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        //
        // Summary:
        //     Gets or sets the template that is used to display the description viewer glyph.
        //
        // Returns:
        //     The template that is used to display the description viewer glyph.
        public ControlTemplate GlyphTemplate
        {
            get { return (ControlTemplate)GetValue(GlyphTemplateProperty); }
            set { SetValue(GlyphTemplateProperty, value); }
        }
        //
        // Summary:
        //     Gets a value that indicates whether the System.Windows.Controls.DescriptionViewer.Target
        //     field data is valid.
        //
        // Returns:
        //     true if the field data is valid; otherwise, false.
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
        }
        //
        // Summary:
        //     Gets or sets the path to the dependency property on the System.Windows.FrameworkElement.DataContext
        //     of the System.Windows.Controls.DescriptionViewer.Target control that this System.Windows.Controls.DescriptionViewer
        //     is associated with.
        //
        // Returns:
        //     The path to the dependency property on the System.Windows.FrameworkElement.DataContext
        //     of the System.Windows.Controls.DescriptionViewer.Target control that this System.Windows.Controls.DescriptionViewer
        //     is associated with.
        public string PropertyPath
        {
            get { return (string)GetValue(PropertyPathProperty); }
            set { SetValue(PropertyPathProperty, value); }
        }
        //
        // Summary:
        //     Gets or sets the control that this System.Windows.Controls.DescriptionViewer
        //     is associated with.
        //
        // Returns:
        //     The control that this System.Windows.Controls.DescriptionViewer is associated
        //     with.
        public FrameworkElement Target
        {
            get { return (FrameworkElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        //
        // Summary:
        //     Gets or sets the style used to display tooltips.
        //
        // Returns:
        //     The style used to display tooltips.
        public Style ToolTipStyle
        {
            get { return (Style)GetValue(ToolTipStyleProperty); }
            set { SetValue(ToolTipStyleProperty, value); }
        }
        //
        // Summary:
        //     Gets a value that indicates whether the control that is the System.Windows.Controls.DescriptionViewer.Target
        //     of the System.Windows.Controls.DescriptionViewer has focus.
        //
        // Returns:
        //     true if the System.Windows.Controls.DescriptionViewer.Target control has focus;
        //     otherwise, false.
        protected bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
        }

        //
        // Summary:
        //     Builds the visual tree for the System.Windows.Controls.DescriptionViewer control
        //     when a new template is applied.
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {

        }
        //
        // Summary:
        //     Reloads the metadata from the System.Windows.Controls.DescriptionViewer.Target
        //     element.
        public virtual void Refresh()
        {

        }
        //
        //// Summary:
        ////     Returns a System.Windows.Automation.Peers.DescriptionViewerAutomationPeer for
        ////     use by the Silverlight automation infrastructure.
        ////
        //// Returns:
        ////     A System.Windows.Automation.Peers.DescriptionViewerAutomationPeer for the System.Windows.Controls.DescriptionViewer
        ////     object.
        //protected override AutomationPeer OnCreateAutomationPeer()
        //{

        //}
    }
}

#endif