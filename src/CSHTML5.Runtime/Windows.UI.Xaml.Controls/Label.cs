

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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents the text label for a control and provides support for access keys.
    /// </summary>
    public partial class Label : ContentControl
    {
        ///// <summary>
        ///// Identifies the System.Windows.Controls.Label.Target dependency property.
        ///// </summary>
        //public static readonly DependencyProperty TargetProperty;

        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Controls.Label class.
        ///// </summary>
        //public Label();

        ///// <summary>
        ///// Gets or sets the element that receives focus when the user presses the label's
        ///// access key.
        ///// </summary>
        //[TypeConverter(typeof(NameReferenceConverter))]
        //public UIElement Target { get; set; }

        ///// <summary>
        ///// Provides an appropriate System.Windows.Automation.Peers.LabelAutomationPeer
        ///// implementation for this control, as part of the WPF infrastructure.
        ///// </summary>
        ///// <returns>The type-specific System.Windows.Automation.Peers.AutomationPeer implementation.</returns>
        //protected override AutomationPeer OnCreateAutomationPeer();
    }
}
