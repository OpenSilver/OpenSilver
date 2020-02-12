
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
