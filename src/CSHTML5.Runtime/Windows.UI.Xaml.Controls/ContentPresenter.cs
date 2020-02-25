﻿
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
    public partial class ContentPresenter : ContentControl
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

