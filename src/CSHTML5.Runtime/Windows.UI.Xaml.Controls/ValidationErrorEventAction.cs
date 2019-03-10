
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
    /// Describes the reason a System.Windows.FrameworkElement.BindingValidationError
    /// event has occurred.
    /// </summary>
    public enum ValidationErrorEventAction
    {
        /// <summary>
        /// A new System.Windows.Controls.ValidationError has occurred.
        /// </summary>
        Added = 0,

        /// <summary>
        /// An existing System.Windows.Controls.ValidationError has been removed.
        /// </summary>
        Removed = 1,
    }
}

