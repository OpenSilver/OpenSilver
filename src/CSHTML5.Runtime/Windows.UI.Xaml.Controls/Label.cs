
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    public class Label : ContentControl
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
