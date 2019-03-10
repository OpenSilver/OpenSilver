
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
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Specifies the type of journal used by the frame.
    /// </summary>
    public enum JournalOwnership
    {
        /// <summary>
        /// If the System.Windows.Controls.Frame control is a top-level frame, it integrates
        /// with the browser journal; otherwise, it maintains its own journal.
        /// </summary>
        Automatic = 0,
        
        /// <summary>
        /// The System.Windows.Controls.Frame maintains its own journal. This option
        /// can be used with any System.Windows.Controls.Frame.
        /// </summary>
        OwnsJournal = 1,
      
        /// <summary>
        /// The System.Windows.Controls.Frame integrates with the browser journal. This
        /// option can be used only with a top-level System.Windows.Controls.Frame; otherwise,
        /// an exception is thrown.
        /// </summary>
        UsesParentJournal = 2,
    }
}