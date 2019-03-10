
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


using System.Windows.Markup;
#if MIGRATION
using System.Windows.Media;
using System.Windows.Media.Animation;
#else

#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Displays the content of a ContentControl.
    /// </summary>
    [ContentProperty("Content")]
    public class ContentPresenter : ContentControl
    {
        //--------------------------------------------------
        // In our simple implementation, a ContentPresenter
        // is like a ContentControl but with automatic
        // TemplateBindings defined for the "Content"
        // and "ContentTemplate" properties (those bindings
        // are added at compile-time).
        //--------------------------------------------------
    }
}

