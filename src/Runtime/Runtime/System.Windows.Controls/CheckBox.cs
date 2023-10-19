
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

using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that a user can select (check) or clear (uncheck). A
    /// CheckBox can also report its value as indeterminate.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <CheckBox Content="Text of the CheckBox." Checked="CheckBox_Checked" Unchecked="CheckBox_Unchecked"/>
    /// </code>
    /// <code lang="C#">
    /// void CheckBox_Checked(object sender, RoutedEventArgs e)
    /// {
    ///     MessageBox.Show("You checked me.");
    /// }
    ///
    /// void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    /// {
    ///     MessageBox.Show("You unchecked me.");
    /// }
    /// </code>
    /// </example>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Indeterminate", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "InvalidFocused", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "Unchecked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Checked", GroupName = "CheckStates")]
    [TemplateVisualState(Name = "Valid", GroupName = "ValidationStates")]
    [TemplateVisualState(Name = "InvalidUnfocused", GroupName = "ValidationStates")]
    public class CheckBox : ToggleButton
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CheckBox"/> class.
        /// </summary>
        public CheckBox()
        {
            DefaultStyleKey = typeof(CheckBox);
        }

        /// <summary>
        /// Returns a <see cref="CheckBoxAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="CheckBoxAutomationPeer"/> for the <see cref="CheckBox"/> object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new CheckBoxAutomationPeer(this);
    }
}
