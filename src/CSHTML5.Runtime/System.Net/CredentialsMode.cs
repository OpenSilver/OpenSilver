
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
    
    public enum CredentialsMode
    {
        /// <summary>
        /// The first request will attempt to use cookies. If it fails, the first request and all the subsequent ones are done without cookies.
        /// </summary>
        Auto,
        /// <summary>
        /// Cookies are not passed
        /// </summary>
        Disabled,
        /// <summary>
        /// Cookies are passed
        /// </summary>
        Enabled
    }
}
