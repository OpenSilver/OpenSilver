
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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes the likelihood that the media engine can play a media source based
    /// on its file type and characteristics.
    /// </summary>
    public enum MediaCanPlayResponse
    {
        /// <summary>
        /// Media engine cannot support the media source.
        /// </summary>
        NotSupported = 0,
        /// <summary>
        /// Media engine might support the media source.
        /// </summary>
        Maybe = 1,
        /// <summary>
        /// Media engine can probably support the media source.
        /// </summary>
        Probably = 2,
    }
}