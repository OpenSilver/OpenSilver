
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

using System.Windows.Controls;

namespace System.Windows.Automation.Peers;

/// <summary>
/// Exposes <see cref="GroupBox"/> types to UI Automation.
/// </summary>
public class GroupBoxAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupBoxAutomationPeer"/> class.
    /// </summary>
    /// <param name="owner">
    /// The <see cref="GroupBox"/> that is associated with this <see cref="GroupBoxAutomationPeer"/>.
    /// </param>
    public GroupBoxAutomationPeer(GroupBox owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore() => nameof(GroupBox);

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Group;

    /// <inheritdoc />
    protected override string GetNameCore()
    {
        string result = base.GetNameCore();
        if (string.IsNullOrEmpty(result))
        {
            if (GetLabeledByCore() is AutomationPeer labelAutomationPeer)
            {
                result = labelAutomationPeer.GetName();
            }

            if (string.IsNullOrEmpty(result))
            {
                result = ((GroupBox)Owner).GetPlainText();
            }
        }

        if (!string.IsNullOrEmpty(result))
        {
            GroupBox groupBox = (GroupBox)Owner;
            if (groupBox.Header is string)
            {
                return AccessText.RemoveAccessKeyMarker(result);
            }
        }

        return result;
    }
}
