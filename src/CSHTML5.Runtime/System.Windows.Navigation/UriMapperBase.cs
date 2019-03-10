
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
    /// Represents the base class for classes that convert a requested uniform resource
    /// identifier (URI) into a new URI based on mapping rules.
    /// </summary>
    public abstract class UriMapperBase
    {
        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Navigation.UriMapperBase
        ///// class.
        ///// </summary>
        //protected UriMapperBase();

        /// <summary>
        /// When overridden in a derived class, converts a requested uniform resource
        /// identifier (URI) to a new URI.</summary>
        /// <param name="uri">The original URI value to be mapped to a new URI.</param>
        /// <returns>A URI to use for the request instead of the value in the uri parameter.</returns>
        public abstract Uri MapUri(Uri uri);
    }
}