
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

namespace System.Net
{
    /// <summary>
    /// Represents the method that will handle the System.Net.WebClient.UploadStringCompleted
    /// event of a System.Net.WebClient.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A System.Net.UploadStringCompletedEventArgs containing event data.</param>
    public delegate void UploadStringCompletedEventHandler(object sender, UploadStringCompletedEventArgs e);
}
