
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

using OpenSilver.Internal;
using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace System.Windows.Controls;

/// <summary>
/// Represents a control that creates a container that has a border and a header for user interface (UI) content.
/// </summary>
public class GroupBox : HeaderedContentControl
{
    static GroupBox()
    {
        FocusableProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        IsTabStopProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBox), new PropertyMetadata(typeof(GroupBox)));
        EventManager.RegisterClassHandler<GroupBox>(AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBox"/> class.
    /// </summary>
    public GroupBox() { }

    /// <summary>
    /// Creates an implementation of <see cref="AutomationPeer"/> for the <see cref="GroupBox"/> control.
    /// </summary>
    /// <returns>
    /// A <see cref="GroupBoxAutomationPeer"/> for the <see cref="GroupBox"/>.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer() => new GroupBoxAutomationPeer(this);

    /// <summary>
    /// Responds when the <see cref="AccessText.AccessKey"/> for the <see cref="GroupBox"/> is pressed.
    /// </summary>
    /// <param name="e">
    /// The event information.
    /// </param>
    protected override void OnAccessKey(AccessKeyEventArgs e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

    private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
    {
        if (!e.Handled && e.Scope is null && e.Target is null)
        {
            e.Target = (GroupBox)sender;
        }
    }
}
