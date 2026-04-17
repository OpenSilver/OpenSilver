
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

// Derived from WPF (dotnet/wpf, MIT license).

using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that creates a container that has a border and a header
    /// for user interface (UI) content.
    /// </summary>
    public class GroupBox : HeaderedContentControl
    {
        static GroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(typeof(GroupBox)));
            FocusableProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
            IsTabStopProperty.OverrideMetadata(typeof(GroupBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBox"/> class.
        /// </summary>
        public GroupBox() { }

        protected override Automation.Peers.AutomationPeer OnCreateAutomationPeer()
            => new Automation.Peers.GroupBoxAutomationPeer(this);
    }
}
